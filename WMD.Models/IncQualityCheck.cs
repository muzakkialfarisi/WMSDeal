using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class IncQualityCheck
    {
        [Key]
        public int DOProductId { get; set; }
        [ForeignKey("DOProductId")]
        public IncDeliveryOrderArrival IncDeliveryOrderArrival { get; set; }
        public int Quantity { get; set; }
        [StringLength(200)]
        public string Note { get; set; } = "";
        [StringLength(200)]
        public string? ProductImage { get; set; }
        [NotMapped]
        public IFormFile? FormProductImage { get; set; }
        public DateTime DateChecked { get; set; }
        [StringLength(50)]
        public string CheckedBy { get; set; } = "";
        [StringLength(20)]
        public string Status { get; set; } = "";
        [StringLength(30)]
        public string NextStatus { get; set; } = "";
    }
}
