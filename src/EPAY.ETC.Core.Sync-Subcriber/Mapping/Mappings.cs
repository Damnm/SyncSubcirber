using AutoMapper;
using EPAY.ETC.Core.Publisher.Common.Options;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Sync;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Models.Configs;

namespace EPAY.ETC.Core.Sync_Subcriber.Mapping
{ 
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<FeeModel, TransactionSyncModel>()                        
                .ReverseMap();
            CreateMap<PaymentModel, TransactionSyncModel>()
               .ReverseMap();
            CreateMap<PaymentStatusModel, TransactionSyncModel>()
                .ReverseMap();
        }

    }
}
