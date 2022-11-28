using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class InvStorageBin
    {
        [Key]
        [StringLength(50),MaxLength(50,ErrorMessage ="Bin Code cannot be longer than 50 characters")]
        [Display(Name ="Bin Code")]
        public string BinCode { get; set; } = "";
        [Display(Name ="Bin Name")]
        [StringLength(100,ErrorMessage ="Bin Name cannot be longer than 100 characters")]
        public string BinName { get; set; } = "";
        [StringLength(50)]
        public string LevelCode { get; set; } = "";
        [ForeignKey("LevelCode")]
        public InvStorageLevel? InvStorageLevel { get; set; }
        
        [StringLength(150)]
        public string? PlanPhotoLocation { get; set; } = "";
        [NotMapped]
        [Display(Name ="Plan Photo")]
        public IFormFile? FormPlanPhoto { get; set; }
        public DateTime DateCreated { get; set; }
        [StringLength(50)]
        public string CreatedBy { get; set; } = "";
        public DateTime DateModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; } = "";
        [Display(Name ="Status")]
        public FlagEnum Flag { get; set; }
        //public ICollection<InvStorageCode>? InvStorageCodes { get; set; }
        public virtual InvStorageCode? InvStorageCode { get; set; }
    }
}
