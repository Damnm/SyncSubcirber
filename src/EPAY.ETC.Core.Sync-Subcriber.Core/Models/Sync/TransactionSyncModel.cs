using EPAY.ETC.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.Sync
{
        public class TransactionSyncModel
        {
            public string? EmployeeId { get; set; }
            public string? LaneInId { get; set; }
            public string? LaneOutId { get; set; }
            public DateTime? LaneInDate { get; set; }
            public DateTime? LaneOutDate { get; set; }
            public Guid? ShiftId { get; set; }
            public string? RFID { get; set; }
            public Guid? CustomVehicleTypeId { get; set; }
            public string? VehicleType {  get; set; }
            public string? PlateNumber { get; set; }
            public string? PlateColour { get; set; }
            public string? LaneInPlateNumberPhotoUrl { get; set; }
            public string? LaneInVehiclePhotoUrl { get; set; }
            public string? LaneOutPlateNumberPhotoUrl { get; set; }
            public string? LaneOutVehiclePhotoUrl { get; set; }
            public string? TicketId { get; set; }
            public string? TicketTypeId { get; set; }
            public double Amount { get; set; }
            public int Duration { get; set; }
            public string? PaymentMethod { get; set; }
            public DateTime PaymentDate { get; set;}
            public string? TransactionId { get; set; }
            public string? TransactionStatus { get; set; }

        }
    }

