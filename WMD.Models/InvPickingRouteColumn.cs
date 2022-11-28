using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class InvPickingRouteColumn
    {
        [Key]
        public int RouteColumn { get; set; }

        public string RouteCode { get; set; } = "";
        [ForeignKey("RouteCode")]
        public InvPickingRoute? InvPickingRoute { get; set; }

        public string ColumnCode { get; set; } = "";
        [ForeignKey("ColumnCode")]
        public InvStorageColumn? InvStorageColumn { get; set; }

        public int Order { get; set; }
    }
}
