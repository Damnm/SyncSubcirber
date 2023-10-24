

HostBuilder builder = new HostBuilder();

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
    config.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
});

builder.ConfigureServices((hostContext, services) =>
{
    SubscriberOptionModel? subscriberOptions = hostContext.Configuration.GetSection("SubscriberConfiguration").Get<SubscriberOptionModel>();
    services.Configure<List<PublishOption>>(hostContext.Configuration.GetSection("PublisherConfigurations"));
    services.AddLogging(logBuilder =>
    {
        logBuilder.ClearProviders();
        logBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
        logBuilder.AddNLog(hostContext.Configuration);
    });

    services.AddScoped<IDeviceStatusService, DeviceStatusService>();

    services.AddRabbitMQCore(hostContext.Configuration);
    services.AddRabbitMQSubscriber();
    services.AddRabbitMQPublisher();
    var multiplexer = Redis.ConnectionMultiplexer.Connect(hostContext.Configuration.GetSection("RedisSettings").GetValue<string>("ConnectionString") ?? "localhost:6379");
    services.AddSingleton(multiplexer.GetDatabase());
    services.AddAutoMapper(typeof(Program));
    var serviceProvider = services.BuildServiceProvider();

    // Load nlog config
    string configFile = $"nlog.{environmentName}.config";
    if (!File.Exists(configFile))
    {
        configFile = $"nlog.config";
    }
    LogManager.Setup().LoadConfigurationFromFile(configFile);

    // Create new Subscriber
    ISubscriberService subscriber = serviceProvider.GetRequiredService<ISubscriberService>();
    IDeviceStatusService deviceStatusService = serviceProvider.GetRequiredService<IDeviceStatusService>();

    subscriber.CreateSubscriber(option =>
    {
        /***** Common options *******/
        option.CreateExchangeQueue = subscriberOptions?.CreateExchangeQueue ?? false;
        option.ExchangeOrQueue = subscriberOptions?.ExchangeOrQueue ?? ExchangeOrQueueEnum.Queue;
        option.AutoAck = subscriberOptions?.AutoAck ?? true;
        /***** End common options *******/

        /***** Exchange options *******/
        if (option.ExchangeOption == null)
            option.ExchangeOption = new EPAY.ETC.Core.Subscriber.Common.Options.ExchangeOption();
        option.ExchangeOption.ExchangeName = subscriberOptions?.ExchangeOption?.ExchangeName ?? string.Empty;
        option.ExchangeOption.ExchangeType = subscriberOptions?.ExchangeOption?.ExchangeType ?? ExchangeTypeEnum.headers;
        option.ExchangeOption.Durable = subscriberOptions?.ExchangeOption?.Durable ?? true;
        option.ExchangeOption.AutoDelete = subscriberOptions?.ExchangeOption?.AutoDelete ?? false;
        option.ExchangeOption.AlternateExchange = subscriberOptions?.ExchangeOption?.AlternateExchange ?? string.Empty;
        /***** End exchange options *******/

        /***** Queue options *******/
        foreach (var queueOption in subscriberOptions?.QueueOptions ?? new List<QueueOption>())
        {
            option.QueueOptions.Add(new EPAY.ETC.Core.Subscriber.Common.Options.QueueOption()
            {
                QueueName = queueOption.QueueName ?? string.Empty,
                RoutingKey = queueOption.RoutingKey,
                BindArguments = queueOption.BindArguments ?? new Dictionary<string, object>(),
                DeadLetterExchange = queueOption.DeadLetterExchange ?? string.Empty,
                DeadLetterRoutingKey = queueOption.DeadLetterRoutingKey ?? string.Empty,
                MessageTTL = queueOption?.MessageTTL ?? -1L,
            });
        }
        /***** End queue options *******/
    });

    Console.WriteLine($"Start program Receive message in env {hostContext.HostingEnvironment} from queue {string.Join(", ", subscriberOptions?.QueueOptions.Select(x => x.QueueName) ?? new List<string>())} and exchange {subscriberOptions?.ExchangeOption?.ExchangeName}");
    Console.WriteLine();

    subscriber.Subscribe(async opt =>
    {
        Console.WriteLine($"Time {DateTime.Now.ToLocalTime()} message {opt.Message}");

        var data = await deviceStatusService.DeviceStatusSubscriberAsync(opt.Message);

        if (data)
            subscriber.Acknowledge(opt.DeliveryTag);
        else
            subscriber.NotAcknowledge(opt.DeliveryTag);
    });
});


var host = builder.Build();

host.Run();