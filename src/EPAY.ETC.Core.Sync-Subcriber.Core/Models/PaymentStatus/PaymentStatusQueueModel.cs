using EPAY.ETC.Core.Models.Enums;
using EPAY.ETC.Core.Models.Fees;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.PaymentStatus
{
    public class PaymentStatusQueueModel
    {
        public PaymentStatusResponseQueueModel Data { set; get; }
    }

    public class PaymentStatusResponseQueueModel
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public DateTime? CreatedDate { set; get; }
        public PaymentModel Payment { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatusEnum Status { get; set; }
        public string? TransactionId { get; set; }
        public string? Reason { get; set; }
    }
}
