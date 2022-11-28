using System.ComponentModel.DataAnnotations;

namespace WMS.Models
{
    public class MasSupplierService
    {
        [Key]
        public int ServiceId { get; set; }
        [Required,StringLength(20),Display(Name ="Supplier Service Code")]
        public string SupplierServiceCode { get; set; } = "";
        [Required,StringLength(150),Display(Name ="Supplier Service Name")]
        public string SupplierServiceName { get; set; } = "";
        [Display(Name = "Status")]
        public FlagEnum Flag { get; set; }
        public ICollection<MasSupplierData>? MasSupplierData { get; set; }

    }
}
