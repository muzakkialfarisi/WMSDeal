using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class InvReturn
    {
        [Key]
        public string ReturnNumber { get; set; } = string.Empty;

        public string ReturnName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string HouseCode { get; set; }
        [ForeignKey("HouseCode")]
        public MasHouseCode? MasHouseCode { get; set; }

        public Guid? TenantId { get; set; }
        [ForeignKey("TenantId")]
        public MasDataTenant? MasDataTenant { get; set; }

        public string? BeautyPicture { get; set; }
        [NotMapped]
        public IFormFile? FormBeautyPicture { get; set; }

        public DateTime DateCreated { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime DateReceived { get; set; }

        public string ReceivedBy { get; set; } = string.Empty;

        public int Flag { get; set; }

        public virtual List<InvReturnProduct>? InvReturnProducts { get; set; } = new List<InvReturnProduct>();
    }
}