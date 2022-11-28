using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class IncRequestPurchase
    {
        [Key]
        public int RequestId { get; set; }
        [Required]
        [Display(Name = "Request Number")]
        [StringLength(100)]
        public string RequestNumber { get; set; }

        public Guid? TenantId { get; set; }
        [ForeignKey("TenantId")]
        public MasDataTenant? MasDataTenant { get; set; }

        [Display(Name = "Tenant")]
        public int? TenantDivisionId { get; set; }
        [ForeignKey("TenantDivisionId")]
        public MasDataTenantDivision? masDataTenantDivision { get; set; }

        [Display(Name = "Warehouse")]
        public int TenantHouseId { get; set; }
        [ForeignKey("TenantHouseId")]
        public MasDataTenantWarehouse? MasDataTenantWarehouse { get; set; }

        
        [Display(Name = "Special Instruction")]
        public string? SpecialInstruction { get; set; } = "";

        [Display(Name = "Date Requested")]
        public DateTime? DateRequested { get; set; }

        [Display(Name = "Date Reviewed")]
        public DateTime? DateReviewed { get; set; }

        [Display(Name = "Date Approved")]
        public DateTime? DateApproved { get; set; }

        [Display(Name = "Requested By")]
        public string? RequestedBy { get; set; }

        [Display(Name = "Reviewed By")]
        public string? ReviewedBy { get; set; } = "";

        [Display(Name = "Approved By")]
        public string? ApprovedBy { get; set; } = "";

        //Canceled
        //Open
        //Apply
        //Reviewed
        //Approved
        //Issued
        //Rejected
        [Display(Name = "Request Status")]
        public string RequestStatus { get; set; } = "";
        public virtual List<IncRequestPurchaseProduct>? IncRequestPurchaseProducts { get; set; } = new List<IncRequestPurchaseProduct>();
    }
}
