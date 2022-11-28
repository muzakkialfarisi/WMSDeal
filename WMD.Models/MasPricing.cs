using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class MasPricing
    {
        [Key]
        public int PriceId { get; set; }
        public Guid TenantId { get; set; }
        [ForeignKey("TenantId")]
        public MasDataTenant MasDataTenant { get; set; }
        public float StorageMin { get; set; }
        public decimal StorageRates { get; set; }
        [StringLength(50)]
        public string StorageRatesType { get; set; }
        public float ReceivingFeeMin { get; set; }
        public decimal ReceivingFeeRates { get; set; }
        [StringLength(50)]
        public string ReceivingFeeRatesType { get; set; }
        public int OutgoingFeeMin { get; set; }
        public decimal OutgoingFeeRates { get; set; }
        [StringLength(50)]
        public string OutgoingFeeRatesType { get; set; }
        public decimal SystemCost { get; set; }
        public float ManagementFee { get; set; }
        public decimal InsuranceFee { get; set; }
        public float PPh { get; set; }
        public FlagEnum Flag { get; set; }

        [ValidateNever]
        public virtual List<MasPricingAdditional>? MasPricingAdditionals { get; set; } = new List<MasPricingAdditional>();
    }
}
