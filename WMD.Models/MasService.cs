using System.ComponentModel.DataAnnotations;

namespace WMS.Models
{
    public class MasService
    {
       [Key]
       public int ServiceId { get; set; }
        [Required(ErrorMessage ="{0} Required"),StringLength(20),Display(Name ="Service Code")]
        public string ServiceCode { get; set; } = "";
        [Required(ErrorMessage = "{0} Required"), StringLength(150),Display(Name ="Service Name")]
        public string ServiceName { get; set; } = "";
        public FlagEnum Flag { get; set; }
    }
}
