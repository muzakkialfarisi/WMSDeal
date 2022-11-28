using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class IncRequestPurchaseProduct
    {
        [Key]
        public int RequestProductId { get; set; }

        [Display(Name = "Product")]
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        [Display(Name = "Product")]
        public MasProductData? MasProductData { get; set; }

        public int Quantity { get; set; } = 0;

        public int? ApprovedQuantity { get; set; } = 0;

        [Display(Name = "Bid Price")]
        public float? BidPrice { get; set; } = 0;

        [Display(Name = "Negotiated Price")]
        public float? NegotiatedPrice { get; set; } = 0;

        [Display(Name = "Final Price")]
        public float FinalPrice { get; set; } = 0;

        [Display(Name = "Expected Arrival")]
        public DateTime ExpArrivalDate { get; set; }

        [StringLength(250)]
        [Display(Name = "Supplier Recomendation")]
        public string? Supplier { get; set; } = "";

        [StringLength(150)]
        public string? Memo { get; set; } = "";

        public string? ApprovedMemo { get; set; }

        [StringLength(50)]
        [Display(Name = "Product Status")]
        public string Status { get; set; } = "";

        public int RequestId { get; set; }
        [ForeignKey("RequestId")]
        public IncRequestPurchase? IncRequestPurchase { get; set; }
    }
}
