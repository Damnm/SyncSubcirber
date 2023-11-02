using EPAY.ETC.Core.Sync_Subcriber.Core.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EPAY.ETC.Core.Sync_Subcriber.Core.Models.LaneTransaction
{
    public class TCPTransactionRequestModel
    {
        [Required(ErrorMessage = "TCPTransactionId required"), StringLength(50)]
        public string TCPTransactionId { get; set; } = string.Empty;
        [StringLength(50)]
        public string CameraId { get; set; } = string.Empty;
        [StringLength(50)]
        public string CameraIP { get; set; } = string.Empty;
        [StringLength(200)]
        public string GateId { get; set; } = string.Empty;
        [StringLength(200)]
        public string LaneId { get; set; } = string.Empty;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DirectionEnum Direction { get; set; }
        [Required(ErrorMessage = "DateTime required")]
        public DateTime DateTime { get; set; }
        public List<string> Photos { get; set; } = new List<string>();
    }
}
