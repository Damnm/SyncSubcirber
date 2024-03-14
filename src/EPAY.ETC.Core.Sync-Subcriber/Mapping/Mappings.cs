using AutoMapper;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;

namespace EPAY.ETC.Core.Sync_Subcriber.Mapping
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<FeeModel, EpayReportFeeModel>()
                .ForPath(e => e.FeeId, act => act.MapFrom(src => src.Id))
                .ForPath(e => e.BlockNumber, act => act.MapFrom(src => src.Block))
                .ForPath(e => e.ExternalEmployeeId, act => act.MapFrom(src => src.EmployeeId))
                .ForPath(e => e.CustomVehicleTypeName, act => act.MapFrom(src => src.CustomVehicleType == null ? null : src.CustomVehicleType.Desc))
                .ForPath(e => e.VehicleCategoryName, act => act.MapFrom(src => src.VehicleCategory == null ? null : src.VehicleCategory.VehicleCategoryName))
                .ForPath(e => e.TicketTypeName, act => act.MapFrom(src => src.TicketType == null ? null : src.TicketType.Name))
                .ReverseMap();
            CreateMap<PaymentModel, EpayReportPaymentModel>()
                .ForPath(e => e.PaymentId, act => act.MapFrom(src => src.Id));
            CreateMap<PaymentStatusModel, EpayReportPaymentStatusModel>().ReverseMap();
            CreateMap<ETCCheckoutModel, EpayReportETCCheckoutModel>().ReverseMap();
            CreateMap<ParkingLogModel, EpayReportParkingLogModel>().ReverseMap();
        }
    }
}
