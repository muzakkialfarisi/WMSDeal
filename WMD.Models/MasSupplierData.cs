using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class MasSupplierData
    {
        [Key]
        public int SupplierId { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = "";

        [StringLength(250)]
        public string Address { get; set; } = "";
        public string KelId { get; set; } = "";
        [ForeignKey("KelId")]
        public MasKelurahan? MasKelurahan { get; set; }
        public Guid? TenantId { get; set; }
        [ForeignKey("TenantId")]
        public MasDataTenant? MasDataTenant { get; set; }

        [StringLength(25), Display(Name="Kode Pos")]
        public string KodePos { get; set; } = "";

        [StringLength(100)]
        public string Email { get; set; } = "";

        [StringLength(100)]
        public string? OfficePhone { get; set; } = "";

        [StringLength(100)]
        public string? HandPhone { get; set; } = "";

        [StringLength(100)]
        public string RekeningNo { get; set; } = "";

        [StringLength(100), Display(Name="Bank Name")]
        public string BankName { get; set; } = "";

        [StringLength(50), Display(Name = "Term Of Payment")]
        public string TermOfPayment { get; set; } = "";

        [Display(Name = "Credit Limit")]
        public decimal? CreditLimit { get; set; }

        public int SupplierTypeId { get; set; }
        [ForeignKey("SupplierTypeId"), Display(Name = "Supplier Type")]
        public MasSupplierType? MasSupplierType { get; set; }

        public int SupplierServiceId { get; set; }
        [ForeignKey("SupplierServiceId"), Display(Name = "Supplier Service")]
        public MasSupplierService? MasSupplierService { get; set; }
        public int IndustryId { get; set; }
        [ForeignKey("IndustryId")]
        public MasIndustry? MasIndustry { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreateDate { get; set; } 
        public DateTime ModifiedDate { get; set; }= DateTime.Now;
        [StringLength(50)]
        public string ModifiedBy { get; set; } = "";

        [Display(Name = "Status")]
        public FlagEnum Flag { get; set; }
        public ICollection<IncPurchaseOrder>? IncPurchaseOrders { get; set; }
    }
}
