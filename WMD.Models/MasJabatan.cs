using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class MasJabatan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int JobPosId { get; set; }
        [Required]
        [StringLength(50),Display(Name ="Code")]
        public string JobPosCode { get; set; } = "";
        [Required]
        [StringLength(150),Display(Name ="Name")]
        public string JobPosName { get; set; } = "";
        [Display(Name ="Division")]
        public int DivId { get; set; }
        [ForeignKey("DivId")]
        [Display(Name ="Division")]
        public MasDivision? MasDivision { get; set; } 
        [Display(Name ="Status")]
        public FlagEnum Flag { get; set; }
        public ICollection<SecUser>? SecUsers { get; set; } 

    }
}
