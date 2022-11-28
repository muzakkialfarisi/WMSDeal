using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WMS.Models
{
    public class MasDivision
    {
        [Key, Display(Name = "Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DivId { get; set; }
        [Required,StringLength(20), Display(Name = "Code")]
        public string DivCode { get; set; } = "";
        [Required,StringLength(150), Display(Name = "Divisi Name")]
        public string DivName { get; set; } = "";
        [Required,StringLength(20), Display(Name = "Directorate")]
        public string? DirCode { get; set; }
        [Required, Display(Name = "Status")]
        public FlagEnum Flag { get; set; }
        [ForeignKey("DirCode")]
        public MasDirectorate? MasDirectorate { get; set; }
        public ICollection<MasJabatan>? MasJabatans { get; set; }

    }
}
