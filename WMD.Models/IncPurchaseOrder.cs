using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class IncPurchaseOrder
    {
        [Key]
        [StringLength(100)]
        [Required(ErrorMessage = "Purchase Number Required")]
        [Display(Name = "Purchase Number")]
        public string PONumber { get; set; } = "";

        [Display(Name = "Tenant")]
        public Guid? TenantId { get; set; }
        [ForeignKey("TenantId")]
        public MasDataTenant? MasDataTenant { get; set; }

        [Display(Name = "Division")]
        public int? TenantDivisionId { get; set; }
        [ForeignKey("TenantDivisionId")]
        public MasDataTenantDivision? masDataTenantDivision { get; set; }

        [Display(Name = "Warehouse")]
        public int TenantHouseId { get; set; }
        [ForeignKey("TenantHouseId")]
        public MasDataTenantWarehouse? MasDataTenantWarehouse { get; set; }

        [Display(Name = "Supplier")]
        public int? SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public MasSupplierData? MasSupplierData { get; set; }

        [StringLength(200)]
        public string? Note { get; set; }

        public float? Discount { get; set; }

        public float? OrderTax { get; set; }

        public int? RequestId { get; set; }
        [ForeignKey("RequestId")]
        public IncRequestPurchase? IncRequestPurchase { get; set; }

        public DateTime? DateCreated { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        public DateTime DateModified { get; set; }

        [StringLength(50)]
        public string? ModifiedBy { get; set; }

        [StringLength(25)]
        public string? Status { get; set; }

        public virtual List<IncPurchaseOrderProduct>? IncPurchaseOrderProducts { get; set; } = new List<IncPurchaseOrderProduct>();
    }
}
