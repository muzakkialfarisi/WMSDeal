using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class OutSalesOrderConsignee
    {
        [Key]
        [StringLength(100)]
        [Required]
        public string OrderId { get; set; } 
        [ForeignKey("OrderId")]
        public OutSalesOrder? OutSalesOrder { get; set; }
        [StringLength(150)]
        public string CneeName { get; set; } = "";
        [StringLength(25)]
        public string CneePhone { get; set; } = "";
        [StringLength(300)]
        public string CneeAddress { get; set; } = "";
        [StringLength(20)]
        public string? KelId { get; set; } 
        [ForeignKey("KelId")]
        public MasKelurahan? MasKelurahan { get; set; }
        [StringLength(150)]
        public string CneeCity { get; set; } = "";
        [StringLength(20)]
        public string OrdZipCode { get; set; } = "";
        [StringLength(100)]
        public string CneeLatitude { get; set; } = "";
        [StringLength(100)]
        public string CneeLongitude { get; set; } = "";

    }
}
