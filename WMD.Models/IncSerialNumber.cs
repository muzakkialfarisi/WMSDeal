using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class IncSerialNumber
    {
        [Key]
        public string SerialId { get; set; }
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public MasProductData? MasProductData { get; set; }
        public int? DOProductId { get; set; }
        [ForeignKey("DOProductId")]
        public IncDeliveryOrderProduct? IncDeliveryOrderProduct { get; set; }
        public int? OrdProductId { get; set; }
        [ForeignKey("OrdProductId")]
        public OutSalesOrderProduct? OutSalesOrderProduct { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}