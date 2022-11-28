using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class IncDeliveryOrderProduct
    {
        [Key]
        public int DOProductId { get; set; }

        [Required(ErrorMessage = "Delivery Order Number Required")]
        [StringLength(100)]
        [Display(Name = "Delivery Order Number")]
        public string DONumber { get; set; } = "";
        [ForeignKey("DONumber")]
        public IncDeliveryOrder? IncDeliveryOrder { get; set; }

        [Display(Name = "Product")]
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        [Display(Name = "Product")]
        public MasProductData? MasProductData { get; set; }

        public int? POProductId { get; set; }
        [ForeignKey("POProductId")]
        [Display(Name = "POProductId")]
        public IncPurchaseOrderProduct? IncPurchaseOrderProduct { get; set; }

        public int Quantity { get; set; } = 1;

        [Display(Name = "Unit Price")]
        public float UnitPrice { get; set; }

        public float SubTotal { get; set; }

        public string? ClosedNote { get; set; } = "";

        //Booked
        //Arrived
        //Puted
        //sold
        [StringLength(20)]
        public string Status { get; set; } = "Open";

        public DateTime DateOfExpired { get; set; }

        public Guid DOProductCode { get; set; } = Guid.NewGuid();
        
        public virtual List<IncItemProduct>? IncItemProducts { get; set; } = new List<IncItemProduct>();
        public virtual List<IncSerialNumber>? IncSerialNumbers { get; set; } = new List<IncSerialNumber>();
        public virtual IncDeliveryOrderArrival? IncDeliveryOrderArrivals { get; set; }
    }
}
