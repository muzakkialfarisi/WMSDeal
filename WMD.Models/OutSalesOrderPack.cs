using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class OutSalesOrderPack
    {
        [Key]
        public int OrdProductId { get; set; }
        [ForeignKey("OrdProductId")]
        public OutSalesOrderProduct? OutSalesOrderProduct { get; set; }
        public DateTime DatePacked { get; set; }
        [StringLength(50)]
        public string PackedBy { get; set; } = "";
        [StringLength(20)]
        public string PackTypeId { get; set; }
        [ForeignKey("PackTypeId")]
        public MasPackingType? MasPackingType { get; set; }

    }
}
