using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class MasPricingAdditional
    {
        [Key]
        public int AddId { get; set; }
        [Required]
        public int PriceId { get; set; }
        [ForeignKey("PriceId")]
        public MasPricing MasPricing { get; set; }
        [StringLength(150)]
        public string AddName { get; set; }
        public int AddMin { get; set; }
        public decimal AddFee { get; set; }
        [StringLength(50)]
        public string AddFeeType { get; set; }
        public FlagEnum Flag { get; set; }

    }
}