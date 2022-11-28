namespace WMS.Models.ViewModels
{
    public class DeliveryOrderViewModel
    {
        public IEnumerable<IncDeliveryOrder>? incDeliveryOrders { get; set; }

        public IncPurchaseOrder? incPurchaseOrder { get; set; }

        public IEnumerable<IncPurchaseOrder>? incPurchaseOrders { get; set; }
    }

    public class DeliveryOrderArrivalViewModel
    {
        public int DOProductId { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public string? ProductImage { get; set; }
        public string? NotaImage { get; set; }
        public DateTime DateArrived { get; set; }
    }

    public class DeliveryOrderUploadViewModel
    {
        public string DONumber { get; set; }
        public string? NotaImage { get; set; }
    }

    public class DeliveryOrderCreateViewModel
    {
        public IncDeliveryOrder? incDeliveryOrder { get; set; }
        public IEnumerable<MasProductData>? masProductDatas { get; set; }
        public IEnumerable<MasProductBundling>? masProductBundlings { get; set; }
    }
}
