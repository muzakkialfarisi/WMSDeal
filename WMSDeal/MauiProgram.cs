using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Plugin.Maui.Audio;
using WMSDeal.ViewModels;
using WMSDeal.ViewModels.Deliveryorder;
using WMSDeal.ViewModels.More;
using WMSDeal.ViewModels.Pickorder;
using WMSDeal.ViewModels.Putaway;
using WMSDeal.ViewModels.Startup;
using WMSDeal.Views;
using WMSDeal.Views.Deliveryorder;
using WMSDeal.Views.More;
using WMSDeal.Views.Pickorder;
using WMSDeal.Views.Putaway;
using WMSDeal.Views.Startup;
using Microsoft.Maui.LifecycleEvents;
using ZXing.Net.Maui.Controls;

namespace WMSDeal
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseBarcodeReader()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("jetbrains-mono.regular.ttf", "Number");
                })

                .ConfigureLifecycleEvents(events =>
                {
#if ANDROID
                    events.AddAndroid(Android => Android
                    .OnActivityResult((activity, requestCode, resultCode, data) => LogEvent(nameof(AndroidLifecycle.OnActivityResult), requestCode.ToString()))
                    .OnStart((activity) => LogEvent(nameof(AndroidLifecycle.OnStart)))
                    .OnCreate((activity, bundle) => LogEvent(nameof(AndroidLifecycle.OnCreate)))
                    .OnBackPressed((activity) => LogEvent(nameof(AndroidLifecycle.OnBackPressed)))
                    .OnResume((activity) => LogEvent(nameof(AndroidLifecycle.OnResume)))
                    .OnStop((activity) => LogEvent(nameof(AndroidLifecycle.OnStop)))
                    );
#endif
                    static bool LogEvent(string eventName, string type = null)
                    {
                        //Shell.Current.DisplayAlert("deal", eventName, "OK");
                        //System.Diagnostics.Debug.WriteLine($"Lifecycle event : {eventName}{(type == null ? string.Empty : $"({type})")}");
                        return true;
                    }
                });


#if DEBUG
        builder.Logging.AddDebug();
#endif
            //Services
            builder.Services.AddSingleton(AudioManager.Current);

            //Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddSingleton<LoadingPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddSingleton<MorePage>();
            builder.Services.AddSingleton<UserProfilePage>();

            builder.Services.AddTransient<ListDeliveryOrderPage>();
            builder.Services.AddTransient<ArrivalProductPage>();
            builder.Services.AddTransient<DeliveryOrderDetailPage>();
            builder.Services.AddTransient<ListArrivalOrderPage>();

            builder.Services.AddTransient<ListPutawayPage>();
            builder.Services.AddTransient<PutawayDetailPage>();
            builder.Services.AddTransient<PutawayProductPage>();
            builder.Services.AddTransient<PutawayproductItemPage>();
            builder.Services.AddTransient<ListSuccessPutawayPage>();

            builder.Services.AddTransient<ListPickOrderPage>();
            builder.Services.AddTransient<ListCurrentPickPage>();
            builder.Services.AddTransient<PickOrderDetailPage>();
            builder.Services.AddTransient<ListSuccessPickPage>();

            builder.Services.AddTransient<StockOpnamePage>();
            builder.Services.AddTransient<HandoverPage>();
            builder.Services.AddTransient<TransferStoragePage>();

            builder.Services.AddTransient<ScanPage>();

            //ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddSingleton<LoadingViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddSingleton<MoreViewModel>();
            builder.Services.AddSingleton<UserProfileViewModel>();

            builder.Services.AddTransient<DeliveryOrderViewModel>();
            builder.Services.AddTransient<DeliveryOrderDetailViewModel>();
            builder.Services.AddTransient<ArrivalProductViewModel>();
            builder.Services.AddTransient<ArrivalDeliveryOrderViewModel>();

            builder.Services.AddTransient<PutawayViewModel>();
            builder.Services.AddTransient<PutawayDetailViewModel>();
            builder.Services.AddTransient<PutawayProductItemViewModel>();
            builder.Services.AddTransient<PutawayProductViewModel>();
            builder.Services.AddTransient<SuccessPutawayViewModel>();

            builder.Services.AddTransient<PickViewModel>();
            builder.Services.AddTransient<PickOrderViewModel>();
            builder.Services.AddTransient<PickOrderDetailViewModel>();
            builder.Services.AddTransient<SuccessPickViewModel>();

            builder.Services.AddTransient<StockOpnameViewModel>();

            builder.Services.AddTransient<ScanViewModel>();

            return builder.Build();
        }
    }
}