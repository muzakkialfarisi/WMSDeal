namespace WMS.Models.ViewModels
{
    public class PickingRouteViewModel
    {
        public InvPickingRoute? invPickingRoute { get; set; }

        public IEnumerable<InvPickingRoute>? invPickingRoutes { get; set; }

        public InvPickingRouteColumn? invPickingRouteColumn { get; set; }

        public List<InvStorageColumn>? invStorageColumns { get; set; }

        public IEnumerable<InvPickingRouteColumn>? invPickingRouteColumns { get; set; }

        public List<InvStorageZone>? invStorageZones { get; set; }
    }
}