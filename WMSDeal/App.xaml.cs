using WMSDeal.Models;
using WMSDeal.Views;
using WMSDeal.Views.Deliveryorder;
using WMSDeal.Views.More;
using WMSDeal.Views.Pickorder;
using WMSDeal.Views.Putaway;
using WMSDeal.Views.Startup;

namespace WMSDeal
{
    public partial class App : Application
    {
        public static UserInfo UserInfo;
        public static string Token;
        public static string LinkUpdate;
        public App()
        {
            InitializeComponent();
            //Routing
            Routing.RegisterRoute("DeliveryOrderDetailPage", typeof(DeliveryOrderDetailPage));
            Routing.RegisterRoute(nameof(ArrivalProductPage), typeof(ArrivalProductPage));
            Routing.RegisterRoute(nameof(PutawayDetailPage), typeof(PutawayDetailPage));
            Routing.RegisterRoute("PickOrderDetailPage", typeof(PickOrderDetailPage));
            Routing.RegisterRoute("PutawayProduct", typeof(PutawayProductPage));
            Routing.RegisterRoute("PutawayProductItem", typeof(PutawayproductItemPage));
            Routing.RegisterRoute("UserProfile", typeof(UserProfilePage));
            Routing.RegisterRoute("StockOpname", typeof(StockOpnamePage));
            Routing.RegisterRoute("Handover", typeof(HandoverPage));
            Routing.RegisterRoute("TransferStorage", typeof(TransferStoragePage));

            Routing.RegisterRoute("ScanBarcode", typeof(ScanPage));

            MainPage = new AppShell();
        }
    }
}