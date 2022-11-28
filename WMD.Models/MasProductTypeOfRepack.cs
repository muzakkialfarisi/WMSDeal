using System.ComponentModel.DataAnnotations;

namespace WMS.Models
{
    public class MasProductTypeOfRepack
    {
        [Key, Required, Display(Name = "Repack Code"), StringLength(25, MinimumLength = 5)]
        public string RepackCode { get; set; } = "";
        [Required, Display(Name = "Repack Name")]
        public string RepackName { get; set; } = "";
        [Required, Display(Name = "Description")]
        public string RepackDescription { get; set; } = "";
        public FlagEnum Flag { get; set; }
    }
}
