using System.ComponentModel.DataAnnotations;
namespace WMS.Models
{
    public class MasRegional
    {
        [Key]
        public int RegionId { get; set; }
        [Required, StringLength(25)]
        public string RegionCode { get; set; } = "";
        [Required(ErrorMessage ="{0} harus diisi"),StringLength(100)]
        [Display(Name ="Region Name")]
        public string? RegionName { get; set; }
        [Display(Name ="Status")]
        public FlagEnum Flag { get; set; }
        public ICollection<MasProvinsi>? MasProvinsis { get; set; }  
       
    }
}
