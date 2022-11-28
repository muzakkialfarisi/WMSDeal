using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class InvPickingRoute
    {
        [Key]
        
        public string RouteCode { get; set; } = "";

        [StringLength(50)]
        public string Name { get; set; } = "";

        [StringLength(25)]
        public string HouseCode { get; set; } = "";
        [ForeignKey("HouseCode")]
        public MasHouseCode? MasHouseCode { get; set; }

        public string Log { get; set; } = "";

        public string CreatedBy { get; set; } = "";

        public DateTime DateCreated { get; set; }

        public string? UpdatedBy { get; set; } = "";

        public DateTime? DateUpdated { get; set; }

        public FlagEnum Flag { get; set; }

        public virtual List<InvPickingRouteColumn>? InvPickingRouteColumns { get; set; } = new List<InvPickingRouteColumn>();
    }
}
