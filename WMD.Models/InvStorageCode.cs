using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class InvStorageCode
    {
        [Key]
        [Required]
        public Guid StorageCode { get; set; }  
        [Display(Name ="Category")]
        public int StorageCategoryId { get; set; }
        [ForeignKey("StorageCategoryId")]
        [Display(Name ="Category")]
        public InvStorageCategory? InvStorageCategory { get; set; }
        [StringLength(50)]
        [Display(Name ="Bin Code")]
        [Required]
        public string BinCode { get; set; }
        [ForeignKey("BinCode")]
        public InvStorageBin? InvStorageBin { get; set; }
        [StringLength(20)]
        [Display(Name ="Size")]
        public string SizeCode { get; set; } = "";
        [ForeignKey("SizeCode")]
        [Display(Name="Category")]
        public InvStorageSize? InvStorageSize { get; set; }
        [Display(Name="Plan Image")]
        public string? PlanPhoto { get; set; }  
        [NotMapped]
        [Display(Name ="Select Storage Plan Image")]
        public IFormFile? FormPlanPhoto { get; set; }
        [StringLength(200)]
        public string Description { get; set; } = "";
        public DateTime DateCreated { get; set; }
        [StringLength(100)]
        public string CreatedBy { get; set; } = "";
        public DateTime DateModified { get; set; }
        [StringLength(100)]
        public string ModifiedBy { get; set; } = "";
        [Display(Name = "Status")]

        //0=Tidak Aktif
        //1=Available
        //2=Booking
        //3=Used
        public int Flag { get; set; } = 1;
        public int Qty { get; set; } = 0;
        public int QtyOrder { get; set; } = 0;
        [StringLength(20)]
        public string Status { get; set; } = "Empty";


        public ICollection<IncItemProduct>? IncItemProducts { get; set; }
    }
}
