using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Enums;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction
{
    public class LaneOutTransactionModel:AdminBaseEntity<Guid>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public new string Name { get; set; } = string.Empty;
        [Required, StringLength(4), Column(TypeName = "varchar")]
        public string LaneId { get; set; } = string.Empty;
        [Required, StringLength(6), Column(TypeName = "varchar")]
        public string ShiftId { set; get; } = string.Empty;
        [Required, StringLength(6), Column(TypeName = "varchar")]
        public string EmployeeId { set; get; } = string.Empty;
        [Required, MaxLength(50), Column(TypeName = "varchar")]
        public string TransactionId { set; get; } = string.Empty;
        public DateTime LaneOutDate { set; get; }
        public DateTime CreatedDate { set; get; } = DateTime.Now;
        [MaxLength(15), Column(TypeName = "varchar")]
        public string? LaneOutRecogPlate { set; get; }
        [MaxLength(50)]
        public string? PlateType { set; get; }
        [MaxLength(50), Column(TypeName = "varchar")]
        public string? TicketId { set; get; }
        [StringLength(6), Column(TypeName = "varchar")]
        public string? VehicleTypeId { set; get; }
        public int? TransactionType { set; get; } = 0;
        public int ChargeAmount { set; get; } = 0;
        public int DurationTime { set; get; } = 0;
        public bool IsManual { set; get; } = false;
        [Column(TypeName = "varchar")]
        public VehicleChargeTypeEnum VehicleTransactionType { set; get; }
        public bool IsParking { set; get; } = false;
        [MaxLength(50), Column(TypeName = "varchar")]
        public string? RFID { set; get; }
        [Column(TypeName = "varchar")]
        public PaymentMethodEnum? PaymentMethod { set; get; }
        [MaxLength(50), Column(TypeName = "varchar")]
        public string? eTicket { set; get; }
        [MaxLength(300)]
        public string? FrontImageURL { set; get; }
        [MaxLength(300)]
        public string? FrontPlateNumberImageURL { set; get; }
        public bool IsOCRSuccessful { set; get; } = false;
        [StringLength(6), Column(TypeName = "varchar")]
        public string? PriorityTypeId { set; get; }
        [StringLength(6), Column(TypeName = "varchar")]
        public string? PeriodTypeId { set; get; }

        [Column(TypeName = "varchar")]
        public ReconciliationActionEnum? ReconciliationResult { set; get; }
        [MaxLength(50), Column(TypeName = "varchar")]
        public string? ReconciliationSupervisionErrorId { set; get; }

        // Lane in transaction info
        [MaxLength(50)]
        public Guid? LaneInTransactionGuiId { set; get; }
        [Column(TypeName = "varchar")]
        public string? LaneInTransactionId { set; get; }
        public DateTime? LaneInDate { set; get; }
        [Column(TypeName = "varchar")]
        public string? LaneInRecogPlate { set; get; }
        public int InvoiceExportStatus { set; get; }
        public int InvoiceExportCount {  set; get; }
        public DateTime InvoiceExportLastTime {  set; get; }
    }
}
