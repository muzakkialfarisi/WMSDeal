using System.ComponentModel.DataAnnotations;
namespace WMS.Models
{
    public class MasDirectorate
    {
        [Key]
        [Required,StringLength(20),Display(Name ="Directorate Code")]
        public string DirCode { get; set; } = "";
        [Required,MaxLength(150),Display(Name ="Directorate Name")]
        public string DirName { get; set; } = "";
        public ICollection<MasDivision>? MasDivisions { get; set; }

    }
}
