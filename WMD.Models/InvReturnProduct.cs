using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class InvReturnProduct
    {
        [Key]
        public int ReturnProductId { get; set; }

        public string ReturnNumber { get; set; }
        [ForeignKey("ReturnNumber")]
        public InvReturn? InvReturn { get; set; }

        public int OrdProductId { get; set; }
        [ForeignKey("OrdProductId")]
        public OutSalesOrderProduct? OutSalesOrderProduct { get; set; }

        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public MasProductData? MasProductData { get; set; }

        public string SKU { get; set; } = string.Empty;

        public DateTime DateOfExpired { get; set; }

        public int Quantity { get; set; }
        public float UnitPrice { get; set; }

        public string Description { get; set; } = string.Empty;

        public string? CloseUpPicture { get; set; }
        [NotMapped]
        public IFormFile? FormCloseUpPicture { get; set; }
    }
}