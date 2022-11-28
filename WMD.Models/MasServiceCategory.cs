using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class MasServiceCategory
    {
        [Key]
        public int ServiceCategoryId { get; set; }
        [Required(ErrorMessage ="{0} Required"),Display(Name ="Name")]
        [StringLength(100,MinimumLength =5)]
        public string ServiceCategoryName { get; set; } = "";
        public int ServiceId { get; set; }
       
        [ForeignKey("ServiceId")]
        public MasService? MasService { get; set; }

        public FlagEnum Flag { get; set; }
    }
}
