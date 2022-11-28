using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class IncDeliveryOrderArrival
    {
        [Key]
        public int DOProductId { get; set; }
        [ForeignKey("DOProductId")]
        public IncDeliveryOrderProduct? IncDeliveryOrderProduct { get; set; }
        public int Quantity { get; set; } = 0;
        [StringLength(200)]
        public string Note { get; set; } = string.Empty;
        [StringLength(200)]
        public string? ProductImage { get; set; }  
        [NotMapped]
        public IFormFile? FormProductImage { get; set; }
        [StringLength(200)]
        public string? NotaImage { get; set; }
        [NotMapped]
        public IFormFile? FormNotaImage { get; set; }
        public DateTime DateArrived { get; set; } = DateTime.Now;
        [StringLength(50)]
        public string ArrivedBy { get; set; } = "";
        public int QtyNotArrived { get; set; } = 0;
        public string NoteNotArrived { get; set; } = string.Empty;

        public virtual List<InvProductPutaway>? InvProductPutaways { get; set; } = new List<InvProductPutaway>();
        public virtual List<IncDeliveryOrderArrivalProduct>? IncDeliveryOrderArrivalProducts { get; set; } = new List<IncDeliveryOrderArrivalProduct>();
    }
}
