using EPAY.ETC.Core.Models.Fees;
using EPAY.ETC.Core.Models.Utils;
using EPAY.ETC.Core.Sync_Subcriber.Core.Constants;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Interface.Processor;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Utils;
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
            return msgType == Constant.MsgTypeIn;
        }
        public async Task<VehicleLaneTransactionRequestModel> ProcessAsync(FeeModel fee, LaneInVehicleModel laneIn)
        {
            VehicleLaneTransactionRequestModel transaction = new VehicleLaneTransactionRequestModel
            {
                LaneInTransaction = new VehicleLaneInTransactionRequestModel()
                {
                    TransactionId = $"Trans{laneIn.Epoch}",
                    StationId = stationId,
                    LaneId = $"{stationId}{int.Parse(laneIn.LaneInId ?? "01"):D2}",
                    ShiftId = "030101",
                    LaneInDate = laneIn.Epoch.ToSpecificDateTime(Constant.AsianTimeZoneName),
                    TCPCheckPoint = string.Empty,
                    VehicleDetails = laneIn.VehicleInfo == null
                        ? new VehicleLaneInDetailRequestModel { RFID = laneIn.RFID }
                        : new VehicleLaneInDetailRequestModel()
                        {
                            RFID = laneIn.RFID,
                            FrontPlateColour = laneIn.VehicleInfo.PlateColour,
                            RearPlateColour = laneIn.VehicleInfo.RearPlateColour,
                            FrontPlateNumber = laneIn.VehicleInfo.PlateNumber,
                            RearPlateNumber = laneIn.VehicleInfo.RearPlateNumber,
                            FrontImage = laneIn.VehicleInfo.VehiclePhotoUrl,
                            FrontPlateNumberImage = laneIn.VehicleInfo.PlateNumberPhotoUrl,
                            RearImage = laneIn.VehicleInfo.VehicleRearPhotoUrl,
                            RearPlateNumberImage = laneIn.VehicleInfo.PlateNumberRearPhotoUrl,
                            ImageExtension = "JPG",
                            VehicleTypeId = laneIn.VehicleInfo.VehicleType?.ConvertVehicleType(),
                        },
                },
                LaneOutTransaction = null
            };

            return transaction;
        }

        public Task<EpayReportTransactionModel?> ProcessEpayReportAsync(FeeModel fee)
        {
            throw new NotImplementedException();
        }
    }
}
