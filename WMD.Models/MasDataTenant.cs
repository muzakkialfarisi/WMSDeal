using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class MasDataTenant
    {
        [Key]
        public Guid TenantId { get; set; }

        public string TenantCode { get; set; }
        [Required(ErrorMessage = "Name of Tenant Required")]
        [StringLength(150)]
        public string Name { get; set; } = "";
        [StringLength(200)]
        public string Address { get; set; } = "";
        [Required]
        [StringLength(20)]
        [Display(Name ="Kelurahan")]
        public string KelId { get; set; } = "";
        [ForeignKey("KelId")]
        public MasKelurahan? MasKelurahan { get; set; }
        [Required]
        [StringLength(20,MinimumLength =5,ErrorMessage ="Zip Code have 5 digits"),MaxLength(5,ErrorMessage ="Zip Code have 5 digits")]
        [Display(Name ="Zip Code")]
        public string KodePos { get; set; } = "";
        [Required(ErrorMessage = "Email Required")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not Valid")]
        [Display(Name="Email Address")]
        public string EmailAddress { get; set; } = "";
        [StringLength(50)]
        [Display(Name = "Office Phone")]
        public string OfficePhone { get; set; } = "";
        [StringLength(50)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = "";
        [StringLength(150)]
        [Display(Name ="Profile Picture")]
        public string? ProfileImageUrl { get; set; } 
        [Display(Name="Profile Picture")]
        [NotMapped]
        public IFormFile? FormProfileImage { get; set; }
        [Display(Name = "Show Price")]
        public bool ShowPrice { get; set; } = true;

        [Display(Name = "Create Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreatedDate { get; set; }
        [StringLength(50)]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; } = "";
        [Display(Name = "Modified Date")]

        public DateTime ModifiedDate { get; set; }
        [StringLength(50)]
        [Display(Name = "Modified Date")]
        public string ModifiedBy { get; set; } = "";
        [Display(Name ="Status")]
        public FlagEnum Flag { get; set; }   
        public ICollection<MasProductData>? MasProductDatas { get; set; }
        public ICollection<IncRequestPurchase>? IncRequestPurchases { get; set; }
        public ICollection<MasSupplierData>? MasSupplierDatas { get; set; }
        public ICollection<IncPurchaseOrder>? IncPurchaseOrders { get; set; }
        public ICollection<MasDataTenantDivision>? MasDataTenantDivisions { get; set; }
        public ICollection<MasDataTenantWarehouse>? MasDataTenantWarehouses { get; set; }
        public ICollection<MasStore>? MasStores { get; set; }
        public ICollection<SecUserTenant>? SecUserTenants { get; set; }
        public ICollection<OutSalesOrder>? OutSalesOrders { get; set; }
        public ICollection<MasInvoicing>? MasInvoicings { get; set; }
    }
}
