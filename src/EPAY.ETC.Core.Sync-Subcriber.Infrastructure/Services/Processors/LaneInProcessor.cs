using EPAY.ETC.Core.Models.Fees;
using EPAY.ETC.Core.Models.Utils;
using EPAY.ETC.Core.Sync_Subcriber.Core.Constrants;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface.Processor;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;
using Microsoft.Extensions.Configuration;

namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Services.Processors
{
    public class LaneInProcessor : ILaneProcesscor
    {
        private readonly IConfiguration _configuration;
        private string stationId = string.Empty;
        public LaneInProcessor(
              IConfiguration configuration)
        {
            _configuration = configuration;
            stationId = _configuration["StationId"];
        }

        public bool IsSupported(string msgType)
        {
            return msgType == Constrant.MsgTypeIn;
        }
        public async Task<VehicleLaneTransactionRequestModel> ProcessAsync(Guid? paymentId, LaneInVehicleModel laneInVehicleModel)
        {
            VehicleLaneTransactionRequestModel transaction = new VehicleLaneTransactionRequestModel
            {
                LaneInTransaction = new VehicleLaneInTransactionRequestModel()
                {
                    TransactionId = $"TRANS{laneInVehicleModel.Epoch}",
                    StationId = stationId,
                    LaneId = laneInVehicleModel.LaneInId,
                    ShiftId = "030101", // feeModel.ShiftId ??
                    LaneInDate = laneInVehicleModel.Epoch.ToSpecificDateTime("SE Asia Standard Time"),
                    TCPCheckPoint = string.Empty,
                    VehicleDetails = new VehicleLaneInDetailRequestModel()
                    {
                        RFID = laneInVehicleModel.RFID,
                        FrontPlateColour = laneInVehicleModel.VehicleInfo.PlateColour,
                        RearPlateColour = laneInVehicleModel.VehicleInfo.PlateColour,
                        FrontPlateNumber = laneInVehicleModel.VehicleInfo.PlateNumber,
                        RearPlateNumber = laneInVehicleModel.VehicleInfo.PlateNumber,
                        FrontImage = laneInVehicleModel.VehicleInfo.VehiclePhotoUrl,
                        FrontPlateNumberImage = laneInVehicleModel.VehicleInfo.PlateNumberPhotoUrl,
                        RearImage = laneInVehicleModel.VehicleInfo.VehicleRearPhotoUrl,
                        RearPlateNumberImage = laneInVehicleModel.VehicleInfo.PlateNumberRearPhotoUrl,
                        ImageExtension = "JPG",
                        VehicleTypeId = laneInVehicleModel.VehicleInfo.VehicleType,
                    },
                },
                LaneOutTransaction = null
            };

            return transaction;
        }
    }
}
