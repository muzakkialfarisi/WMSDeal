using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
 
    public class SecProfile
    {
        [Key,Display(Name ="Code")]
        [Required ]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProfileId { get; set; }
        [Required(ErrorMessage = "Nama Profile harus diisi")]
        [StringLength(50, ErrorMessage = "{0} tidak boleh lebih (1) karakter")]
        [Display(Name = "Profile Name")]
        public string ProfileName { get; set; } = "";
        [Display(Name = "Description")]
        [Required(ErrorMessage = "{0} harus diisi")]
        [StringLength(250, ErrorMessage = "{0} tidak boleh lebih (1) karakter")]
        public string? Description { get; set; }
        [Display(Name = "Date Created")]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}")]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; } = DateTime.Now;
        [Display(Name = "Created by")]
        [StringLength(25)]
        public string? CreatedBy { get; set; }
        public DateTime DateModified { get; set; } = DateTime.Now;
        [StringLength(50)] 
        public string? ModifiedBy { get; set; }

        [Display(Name = "Status")]
        public FlagEnum Flag { get; set; } = FlagEnum.Active;
    }
}
