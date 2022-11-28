using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class InvStorageSection
    {
        [Key]
        [StringLength(20),MaxLength(20,ErrorMessage ="Section Code cannot be longer than 20 characters")]
        [Display(Name ="Section Code")]
        public string SectionCode { get; set; } = "";
        [StringLength(100,ErrorMessage ="Section Name cannot be longer than 100 characters")]
        [Display(Name ="Section Name")]
        public string SectionName { get; set; } = "";
        [StringLength(20)]
        [Required]
        [Display(Name ="Row")]
        public string RowCode { get; set; } = "";
        [ForeignKey("RowCode")]
        [Display(Name ="Row")]
        public InvStorageRow? InvStorageRow { get; set; }
        [Display(Name ="Status")]
        public FlagEnum Flag { get; set; }
        public DateTime DateCreated { get; set; }
        [StringLength(50)]
        public string CreatedBy { get; set; } = "";
        public DateTime DateModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; } = "";
        public ICollection<InvStorageLevel>? InvStorageLevels { get; set; }
    }
}
