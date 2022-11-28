namespace WMS.Models.ViewModels
{
    public class PurchaseOrderViewModel
    {
        public IncRequestPurchase? IncRequestPurchase { get; set; }
        public IEnumerable<IncRequestPurchase>? IncRequestPurchases { get; set; }
        public IncRequestPurchaseProduct? IncRequestPurchaseProduct { get; set; }
        public IncPurchaseOrder? IncPurchaseOrder { get; set; }
    }
}