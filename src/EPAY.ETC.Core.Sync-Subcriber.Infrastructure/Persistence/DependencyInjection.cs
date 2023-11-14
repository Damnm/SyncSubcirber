using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface.Processor;
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
            services.AddTransient<ISyncSubcriberService, SyncSubcriberService>();
            services.AddTransient<ILaneOutProcesscor, LaneOutProcessor>();
            return services;
        }
    }
}
