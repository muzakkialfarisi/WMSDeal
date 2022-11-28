using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class InvStorageLevel
    {
        [Key]
        [StringLength(50),MaxLength(50,ErrorMessage ="Level Code cannot be longer than 50 characters")]
        [Display(Name ="Level Code")]
        public string LevelCode { get; set; } = "";
        [StringLength(100,ErrorMessage ="Level Name cannot be longer than 100 characters")]
        [Display(Name ="Level Name")]
        public string LevelName { get; set; } = "";
        [StringLength(150), Display(Name = "Levels Plan Photo")]
        public string? LevelPlanPhoto { get; set; }
        [NotMapped]
        public IFormFile? FormLevelPlanPhoto { get; set; }
        [Display(Name ="Status")]
        public FlagEnum Flag { get; set; }
        [StringLength(20)]
        [Display(Name ="Colum Code")]
        public string? ColumnCode { get; set; } 
        [ForeignKey("ColumnCode")]
        public InvStorageColumn? InvStorageColumn { get; set; }
        [StringLength(20)]
        [Display(Name ="Section Code")]
        public string? SectionCode { get; set; } 
        [ForeignKey("SectionCode")]
        public InvStorageSection? InvStorageSection { get; set; }
        public DateTime DateCreated { get; set; }
        [StringLength(50)]
        public string CreatedBy { get; set; } = "";
        public DateTime DateModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; } = "";
        //public ICollection<InvStorageBin>? InvStorageBins { get; set; }

        public List<InvStorageBin> InvStorageBins { get; set; } = new List<InvStorageBin>();
    }
}
