using System.ComponentModel.DataAnnotations;

namespace WMS.Models
{
    public class MasPlatform
    {
        [Key]
        public int PlatformId { get; set; }

        [StringLength(150)]
        public string Name { get; set; } = "";

        [StringLength(25)]
        public string Code { get; set; } = "";

        [Display(Name = "Status")]
        public FlagEnum Flag { get; set; }
    }
}