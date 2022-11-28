using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class InvStockOpnameProduct
    {
        [Key]
        public string OpnameProductId { get; set; } = Guid.NewGuid().ToString();

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public MasProductData? MasProductData { get; set; }

        public string? OpnameId { get; set; }
        [ForeignKey("OpnameId")]
        public InvStockOpname? InvStockOpname { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public DateTime DateOpname { get; set; }

        public int SystemQty { get; set; }

        public int ExpiredQty { get; set; }

        public int BrokenQty { get; set; }

        public string BrokenDescription { get; set; } = string.Empty;

        public int StockQty { get; set; }

        public int DiscrepancyQty { get; set; }

        public string Log { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        [NotMapped]
        public IFormFile? FormImageUrl { get; set; }

        public int Status { get; set; }
    }
}