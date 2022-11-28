using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class InvSalesOrderPick
    {
        [Key]
        public int Id { get; set; }

        public int? OrdProductId { get; set; }
        [ForeignKey("OrdProductId")]
        public OutSalesOrderProduct? OutSalesOrderProduct { get; set; }
        public Guid StorageCode { get; set; }
        [ForeignKey("StorageCode")]
        public InvStorageCode? InvStorageCode { get; set; }

        public int QtyPick { get; set; }
    }
}
