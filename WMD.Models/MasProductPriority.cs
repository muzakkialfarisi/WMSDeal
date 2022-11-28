using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class MasProductPriority
    {
        [Key, Required, Display(Name = "Cargo Priority Code"), StringLength(25, MinimumLength = 2)]
        public string CargoPriorityCode { get; set; } = "";

        [Required, Display(Name = "Cargo Priority Name")]
        public string CargoPriorityName { get; set; } = "";

        [Required, Display(Name = "Description")]
        public string CargoPriorityDescription { get; set; } = "";
        public FlagEnum Flag { get; set; }

    }
}
