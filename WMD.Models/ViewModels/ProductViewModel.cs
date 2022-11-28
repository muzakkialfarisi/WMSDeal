namespace WMS.Models.ViewModels
{
    public class ProductMonitoringViewModel
    {
        public MasProductData? masProductData { get; set; }

        public List<InvProductHistory>? invProductHistories { get; set; }

    }
    public class ProductMonitoringToExcel
    {
        public string Id { get; set; }
        public int No { get; set; }
        public string BeautyPicture { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public string ProductCondition { get; set; }
        public string Tenant { get; set; }
        public string SerialNumber { get; set; }
        public string HouseName { get; set; }
        public int Start_Stock { get; set; }
        public int Incoming { get; set; }
        public int Outgoing { get; set; }
        public int End_Stock { get; set; }
        public string UOM { get; set; }
        public List<InvProductHistory>? invProductHistories { get; set; } = new List<InvProductHistory>();
    }

    public class ProductBundlingCreateViewModel
    {
        public MasProductBundling? masProductBundling { get; set; }
        public IEnumerable<MasProductData>? masProductDatas { get; set; }
    }
}
