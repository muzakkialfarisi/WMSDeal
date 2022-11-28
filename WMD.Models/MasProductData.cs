
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class MasProductData
    {
        [Key]
        public int ProductId { get; set; }

        public Guid ProductCode { get; set; } = Guid.NewGuid();

        [Display(Name = "Tenant")]
        public Guid TenantId { get; set; }
        [ForeignKey("TenantId")]
        public MasDataTenant? MasDataTenant { get; set; }

        public string ProductLevel { get; set; } = "";

        public string SKU { get; set; } = "";

        [Required(ErrorMessage = "Product Name Required ")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = "";

        [Display(Name = "Friendly Name")]
        public string? FriendlyName { get; set; }
        [Display(Name = "Brand Name")]
        public string? BrandName { get; set; }

        public string? BeautyPicture { get; set; }
        [NotMapped]
        [Display(Name = "Beautiful Picture")]
        public IFormFile? FormBeautyPicture { get; set; }

        public string? CloseUpPicture { get; set; }
        [NotMapped]
        [Display(Name = "CloseUp Picture")]
        public IFormFile? FormCloseUpPicture { get; set; }

        [Display(Name = "Actual Weight")]
        public int ActualWeight { get; set; }
        [Display(Name = "Items Per Box")]
        public int? Ipb { get; set; }
        public int Panjang { get; set; }
        public int Lebar { get; set; }
        public int Tinggi { get; set; }
        [Display(Name = "Volumetric Weight")]
        public int VolWight { get; set; }
        [Display(Name = "Storage Period")]
        public int Storageperiod { get; set; }
        [Display(Name = "Safety Stock")]
        public int SafetyStock { get; set; }
        [Display(Name = "Purchased Price")]
        public float PurchasePrice { get; set; }
        [Display(Name = "Selling Price")]
        public float SellingPrice { get; set; }
        public string? Description { get; set; }
        public bool Resellable { get; set; }
        [Display(Name = "Resellable Price")]
        public float? ResellablePrice { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Storage Method")]
        public string StorageMethod { get; set; } = "";
        [Display(Name = "Packaging")]
        public int PackagingId { get; set; }
        [ForeignKey("PackagingId")]
        public MasProductPackaging? MasProductPackaging { get; set; }
        public bool RePackaging { get; set; }
        [Display(Name = "Type Of Repack")]
        public string? TypeOfRepackCode { get; set; } = "";
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public InvStorageCategory? InvStorageCategory { get; set; }
        [Display(Name = "Cargo Priority")]
        public string CargoPriorityCode { get; set; } = "";
        [StringLength(250)]
        [Display(Name = "Supplier Recomendation")]
        public string? Supplier { get; set; } = "";
        [StringLength(50)]
        [Display(Name = "Unit of Product")]
        public string Unit { get; set; } = "";
        public string SizeCode { get; set; } = "";
        [ForeignKey("SizeCode")]
        public InvStorageSize? InvStorageSize { get; set; }
        public string ZoneCode { get; set; } = "";
        [ForeignKey("ZoneCode")]
        public InvStorageZone? InvStorageZone { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = "";
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public string SerialNumber { get; set; } = "";
        [Display(Name = "Status")]
        public FlagEnum Flag { get; set; }
        public string ProductCondition { get; set; } = "";

        public virtual List<InvProductStock>? InvProductStocks { get; set; } = new List<InvProductStock>();
        public virtual List<IncDeliveryOrderProduct>? IncDeliveryOrderProducts { get; set; } = new List<IncDeliveryOrderProduct>();
        public virtual List<OutSalesOrderProduct>? OutSalesOrderProducts { get; set; } = new List<OutSalesOrderProduct>();
        public virtual List<IncSerialNumber>? IncSerialNumbers { get; set; } = new List<IncSerialNumber>();
    }
}