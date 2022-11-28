using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class InvProductPutaway
    {
        [Key]
        public int Id { get; set; }
        public int DOProductId { get; set; }
        [ForeignKey("DOProductId")]
        public IncDeliveryOrderArrival IncDeliveryOrderArrival { get; set; }
        public Guid StorageCode { get; set; }  
        [ForeignKey("StorageCode")]
        public InvStorageCode InvStorageCode { get; set; }
        public int Quantity { get; set; } = 0;
        public int QtyStock { get; set; } = 0;
        public DateTime DatePutaway { get; set; } = DateTime.Now;
        [StringLength(50)]
        public string PutBy { get; set; } = string.Empty;
    }
}
