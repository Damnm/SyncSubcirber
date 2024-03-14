using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Repositories;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Processor;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Persistence.Repositories;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services.Processors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)//, IWebHostEnvironment environment)
        {
            //Add Services

            services.AddHttpClient<ISyncSubcriberService, SyncSubcriberService>(client =>
            {
            }).ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            });

            services.AddTransient<IFeeRepository, FeeRepository>();
            services.AddTransient<ILaneProcesscor, LaneOutProcessor>();
            services.AddTransient<ILaneProcesscor, LaneInProcessor>();

            services.AddTransient<IImageService, ImageService>();

            return services;
        }
    }
}
