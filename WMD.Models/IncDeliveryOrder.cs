using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class IncDeliveryOrder
    {
        [Key]
        [Required(ErrorMessage = "Delivery Order Rquired")]
        [StringLength(100, ErrorMessage = "Delivery Number cannot be longer than 100 characters")]
        [Display(Name = "Delivery Order Number")]
        public string DONumber { get; set; } = "";

        public string DOSupplier { get; set; } = "";

        [StringLength(100)]
        [Display(Name = "Purchase Order Number")]
        public string? PONumber { get; set; }
        [ForeignKey("PONumber")]
        public IncPurchaseOrder? IncPurchaseOrder { get; set; }

        [Display(Name = "Tenant")]
        public Guid? TenantId { get; set; }
        [ForeignKey("TenantId")]
        public MasDataTenant? MasDataTenant { get; set; }

        [Display(Name = "Warehouse")]
        [StringLength(25)]
        public string? HouseCode { get; set; }
        [ForeignKey("HouseCode")]
        public MasHouseCode? MasHouseCode { get; set; }

        [Display(Name = "Supplier")]
        public int? SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public MasSupplierData? MasSupplierData { get; set; }

        [StringLength(200)]
        public string? Note { get; set; } = "";

        public float? ShippingCost { get; set; }

        [Display(Name = "Courier")]
        public int? DeliveryCourierId { get; set; }
        [ForeignKey("DeliveryCourierId")]
        public MasDeliveryOrderCourier? MasDeliveryOrderCourier { get; set; }

        public DateTime? DateDelivered { get; set; }

        public DateTime? DateArrived { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now.ToLocalTime();

        [StringLength(50)]
        public string? CreatedBy { get; set; }
        [StringLength(20)]
        public string? Status { get; set; } = "Open";

        public string? KTP { get; set; } = string.Empty;
        public string? Name { get; set; } = string.Empty;

        public virtual List<IncDeliveryOrderProduct>? IncDeliveryOrderProducts { get; set; } = new List<IncDeliveryOrderProduct>();
    }
}
