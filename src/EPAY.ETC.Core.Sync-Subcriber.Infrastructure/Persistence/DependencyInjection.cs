using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services;
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
            services.AddTransient<ISyncService,SyncServices>();
            return services;
        }
    }
}
