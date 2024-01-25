using EPAY.ETC.Core.Models.Fees;
using EPAY.ETC.Core.Models.Utils;
using EPAY.ETC.Core.Sync_Subcriber.Core.Constants;
using EPAY.ETC.Core.Sync_Subcriber.Core.Interface.Services.Processor;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction;
using EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Utils;
using Microsoft.Extensions.Configuration;
using FeeModel = EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities.FeeModel;
using FeeRequestModel = EPAY.ETC.Core.Models.Fees.FeeModel;

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
        public async Task<VehicleLaneTransactionRequestModel?> ProcessAsync(FeeRequestModel feeRequest, FeeModel feeEntity, LaneInVehicleModel laneInRequest)
        {
            VehicleLaneTransactionRequestModel transaction = new VehicleLaneTransactionRequestModel
            {
                LaneInTransaction = new VehicleLaneInTransactionRequestModel()
                {
                    TransactionId = $"Trans{laneInRequest.Epoch}",
                    StationId = stationId,
                    LaneId = $"{stationId}{int.Parse(laneInRequest.LaneInId ?? "01"):D2}",
                    ShiftId = "030101",
                    LaneInDate = laneInRequest.Epoch.ToSpecificDateTime(Constant.AsianTimeZoneName),
                    TCPCheckPoint = string.Empty,
                    VehicleDetails = laneInRequest.VehicleInfo == null
                        ? new VehicleLaneInDetailRequestModel { RFID = laneInRequest.RFID }
                        : new VehicleLaneInDetailRequestModel()
                        {
                            RFID = laneInRequest.RFID,
                            FrontPlateColour = laneInRequest.VehicleInfo.PlateColour,
                            RearPlateColour = laneInRequest.VehicleInfo.RearPlateColour,
                            FrontPlateNumber = laneInRequest.VehicleInfo.PlateNumber,
                            RearPlateNumber = laneInRequest.VehicleInfo.RearPlateNumber,
                            FrontImage = laneInRequest.VehicleInfo.VehiclePhotoUrl,
                            FrontPlateNumberImage = laneInRequest.VehicleInfo.PlateNumberPhotoUrl,
                            RearImage = laneInRequest.VehicleInfo.VehicleRearPhotoUrl,
                            RearPlateNumberImage = laneInRequest.VehicleInfo.PlateNumberRearPhotoUrl,
                            ImageExtension = "JPG",
                            VehicleTypeId = laneInRequest.VehicleInfo.VehicleType?.ConvertVehicleType(),
                        },
                },
                LaneOutTransaction = null
            };

            return transaction;
        }

        public Task<EpayReportTransactionModel?> ProcessEpayReportAsync(FeeRequestModel feeRequest, FeeModel feeEntity)
        {
            throw new NotImplementedException();
        }
    }
}
