namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.ImageEmbedInfo
{
    public class ImageEmbedInfoRequest
    {
        public Guid ReferenceId { get; set; }
        public string AirportId { get; set; } = string.Empty;
        public string TerminalId { get; set; } = string.Empty;
        public DateTime LaneOutDateTime { get; set; }
        public string VehicleType { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public string TicketType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string RFID { get; set; } = string.Empty;
        public string ImageId { get; set; } = string.Empty;
    }
}
