using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class SecAuditTrail
    {
        [Key]
        public int Id { get; set; }
        [Required,StringLength(150)]
        public string EventName { get; set; } = "";
        [StringLength(150)]
        public string EventMenu { get; set; } = "";
        [StringLength(100)]
        public string EventFunction { get; set; } = "";
        [DataType(DataType.Date),DisplayFormat(DataFormatString ="{0:dd-MM-yyyy")]
        public DateTime EventDate { get; set; }
        [StringLength(250)]
        public string EventDesctiption { get; set; } = "";
        [StringLength(50)]
        public string IpAddress { get; set; } = "";
        [Required,StringLength(50,MinimumLength =5)]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public SecUser? SecUser { get; set; }

    }
}
