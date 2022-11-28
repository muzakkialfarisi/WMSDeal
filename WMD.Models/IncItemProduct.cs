using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class IncItemProduct
    {
        [Key]
        [StringLength(100)]
        public string IKU { get; set; } = "";
        [Required]
        public int DOProductId { get; set; }
        [ForeignKey("DOProductId")]
        public IncDeliveryOrderProduct? IncDeliveryOrderProduct { get; set; }
        [Required]
        public Guid StorageCode { get; set; }
        [ForeignKey("StorageCode")]
        public InvStorageCode? InvStorageCode { get; set; }
        [StringLength(250)]
        public string? Note { get; set; }
        public DateTime DateCreated { get; set; }
        [StringLength(50)]
        public string CreatedBy { get; set; } = "";

        //sttaus 0 cenc
        //status 1 open
        //status 2 booked
        //status 3 arrived
        //status 4 puted
        //sttaus 5 sold
        //status 6 picked
        [Display(Name ="Status Inventory")]
        public int Status { get; set; } = 1;
        public DateTime? DateArrived { get; set; }
        [StringLength(50)]
        public string? ArrivedBy { get; set; }
        public DateTime DatePutedAway { get; set; }
        [StringLength(50)]
        public string? PutedAwatBy { get; set; }
    }
}
