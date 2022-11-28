using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class IncDeliveryOrderArrivalProduct
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int? DOProductId { get; set; }
        [ForeignKey("DOProductId")]
        public IncDeliveryOrderArrival? IncDeliveryOrderArrival { get; set; }
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public MasProductData? MasProductData { get; set; }
        public int Quantity { get; set; } = 0;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string Note { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}