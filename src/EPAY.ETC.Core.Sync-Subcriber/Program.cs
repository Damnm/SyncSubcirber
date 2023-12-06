using EPAY.ETC.Core.Models.Constants;
using EPAY.ETC.Core.Publisher.DependencyInjectionExtensions;
using EPAY.ETC.Core.RabbitMQ.Common.Enums;
using EPAY.ETC.Core.RabbitMQ.DependencyInjectionExtensions;
using EPAY.ETC.Core.Subscriber.Common.Options;
using EPAY.ETC.Core.Subscriber.DependencyInjectionExtensions;
using EPAY.ETC.Core.Subscriber.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface.Processor;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.Configs;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Persistence;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Persistence.Context;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services.Processors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;
using NLog;
using NLog.Extensions.Logging;
using System.Security.Authentication;
using System.Text;

HostBuilder builder = new HostBuilder();

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
    config.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
});

builder.ConfigureServices(async (hostContext, services) =>
{
    SubscriberOptionModel? subscriberOptions = hostContext.Configuration.GetSection("SubscriberConfiguration").Get<SubscriberOptionModel>();

    services.AddLogging(logBuilder =>
    {
        logBuilder.ClearProviders();
        logBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
        logBuilder.AddNLog(hostContext.Configuration);
    });

    services.AddRabbitMQCore(hostContext.Configuration);
    services.AddRabbitMQSubscriber();

    // Infrastructure config
    services.AddInfrastructure(hostContext.Configuration);
    services.AddAutoMapper(typeof(Program));

    services.AddDbContext<CoreDbContext>(
        opt => opt.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));

    var serviceProvider = services.BuildServiceProvider();

    // Load nlog config
    string configFile = $"nlog.{environmentName}.config";
    if (!File.Exists(configFile))
    {
        configFile = $"nlog.config";
    }
    LogManager.Setup().LoadConfigurationFromFile(configFile);

    ISubscriberService subscriber = serviceProvider.GetRequiredService<ISubscriberService>();
    ILaneProcesscor laneProcesscor = serviceProvider.GetRequiredService<ILaneProcesscor>();

    ISyncSubcriberService syncSubcriberService = serviceProvider.GetRequiredService<ISyncSubcriberService>();
    ILogger<Program> _logger = serviceProvider.GetRequiredService<ILogger<Program>>();

    subscriber.CreateSubscriber(option =>
    {
        /***** Common options *******/
        option.CreateExchangeQueue = subscriberOptions?.CreateExchangeQueue ?? false;
        option.ExchangeOrQueue = subscriberOptions?.ExchangeOrQueue ?? ExchangeOrQueueEnum.Queue;
        option.AutoAck = subscriberOptions?.AutoAck ?? true;
        option.PrefetchCount = 1;
        /***** End common options *******/

        /***** Exchange options *******/
        if (option.ExchangeOption == null)
            option.ExchangeOption = new ExchangeOption();

        option.ExchangeOption.ExchangeName = subscriberOptions?.ExchangeOption?.ExchangeName ?? string.Empty;
        option.ExchangeOption.ExchangeType = subscriberOptions?.ExchangeOption?.ExchangeType ?? ExchangeTypeEnum.headers;
        option.ExchangeOption.Durable = subscriberOptions?.ExchangeOption?.Durable ?? true;
        option.ExchangeOption.AutoDelete = subscriberOptions?.ExchangeOption?.AutoDelete ?? false;
        option.ExchangeOption.AlternateExchange = subscriberOptions?.ExchangeOption?.AlternateExchange ?? string.Empty;
        /***** End exchange options *******/

        /***** Queue options *******/
        string laneId = Environment.GetEnvironmentVariable(CoreConstant.ENVIRONMENT_LANE_OUT) ?? "1";
        var queueOptions = subscriberOptions.QueueOptions.Select(x =>
        {
            if (laneId != "1")
            {
                string newQueueName = $"{x.QueueName}_{laneId}";
                x.DeadLetterRoutingKey = x.DeadLetterRoutingKey.Replace(x.QueueName, newQueueName);
                x.QueueName = newQueueName;

                if (x.BindArguments.ContainsKey(CoreConstant.RABBIT_HEADER_PROP_LANEID))
                    x.BindArguments[CoreConstant.RABBIT_HEADER_PROP_LANEID] = Environment.GetEnvironmentVariable(CoreConstant.ENVIRONMENT_LANE_OUT) ?? x.BindArguments[CoreConstant.RABBIT_HEADER_PROP_LANEID];
            }

            return x;
        });

        foreach (var queueOption in subscriberOptions?.QueueOptions ?? new List<QueueOption>())
        {
            option.QueueOptions.Add(new QueueOption()
            {
                QueueName = queueOption.QueueName ?? string.Empty,
                RoutingKey = queueOption.RoutingKey,
                BindArguments = queueOption.BindArguments ?? new Dictionary<string, object>(),
                DeadLetterExchange = queueOption.DeadLetterExchange ?? string.Empty,
                DeadLetterRoutingKey = queueOption.DeadLetterRoutingKey ?? string.Empty,
                //MessageTTL = queueOption?.MessageTTL ?? -1L,
            });
        }
        /***** End queue options *******/
    });

    _logger.LogInformation($"Start program Receive message in env {hostContext.HostingEnvironment} from queue {string.Join(", ", subscriberOptions?.QueueOptions.Select(x => x.QueueName) ?? new List<string>())} and exchange {subscriberOptions?.ExchangeOption?.ExchangeName}");

    subscriber.Subscribe(async opt =>
    {
        string msgType = string.Empty;

        if (opt.Headers != null)
        {
            if (opt.Headers.TryGetValue("MsgType", out object? bytes) && bytes != null)
            {
                msgType = Encoding.UTF8.GetString((byte[])bytes);
            }
        }

        string laneId = string.Empty;

        if (opt.Headers != null)
        {
            if (opt.Headers.TryGetValue(CoreConstant.ENVIRONMENT_LANE_OUT, out object? laneIdBytes) && laneIdBytes != null)
            {
                laneId = Encoding.UTF8.GetString((byte[])laneIdBytes);
                Environment.SetEnvironmentVariable(CoreConstant.ENVIRONMENT_LANE_OUT, laneId);
            }
        }

        Console.WriteLine($"Time {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fffff")} {msgType} {opt.DeliveryTag} - Start process\r\n");
        Console.WriteLine($"{opt.Message}\r\n");

        var result = await syncSubcriberService.SyncSubcriber(opt.Message, msgType);

        await Task.Yield();

        Console.WriteLine($"Time {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fffff")} {msgType} {opt.DeliveryTag} - Processed done\r\n");

        if (result)
            subscriber.Acknowledge(opt.DeliveryTag);
        else
            subscriber.NotAcknowledge(opt.DeliveryTag);
    });
});


var host = builder.Build();

try
{
host.Run();

}catch  (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

