using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WMS.Models
{
    public class MasCheckPoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CheckPointId { get; set; }
        [Required,StringLength(20)]
        public string CheckPointCode { get; set; } = "";
        [Required,StringLength(100)]
        public string CheckPointName { get; set; } = "";
        [StringLength(150)]
        public string CheckPointDescription { get; set; } = "";
        public FlagEnum Flag { get; set; }
    }
}
