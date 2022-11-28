using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class IncPurchaseOrderProduct
    {
        [Key]
        public int POProductId { get; set; }

        [Required(ErrorMessage = "Purchase Number Required")]
        [StringLength(100)]
        [Display(Name = "Purchase Number")]
        public string PONumber { get; set; } = "";
        [ForeignKey("PONumber")]
        public IncPurchaseOrder? IncPurchaseOrder { get; set; }

        [Display(Name = "Product")]
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        [Display(Name = "Product")]
        public MasProductData? MasProductData { get; set; }

        public int Quantity { get; set; }

        public int DOQuantity { get; set; } = 0;

        [Display(Name = "Unit Price")]
        public float UnitPrice { get; set; }

        public float SubTotal { get; set; }

        public DateTime ExpArrivalDate { get; set; }

        [StringLength(200)]
        public string SpecialInstruction { get; set; } = "";

        public string? ClosedNote { get; set; } = "";

        [StringLength(25)]
        public string? Status { get; set; } = "";

    }
}