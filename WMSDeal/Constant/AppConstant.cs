using System.Net;
using WMSDeal.Views;
using WMSDeal.Views.Deliveryorder;
using WMSDeal.Views.Pickorder;
using WMSDeal.Views.Putaway;

namespace WMSDeal.Constant
{
    public class AppConstant
    {
        //public const string BaseUrl = "https://uat.wmsdeal.com";
        public const string BaseUrl = "https://app.wmsdeal.com";

        public async static Task AddFlyoutMenusDetails()
        {
            //AppShell.Current.FlyoutHeader = new FlyoutHeaderControl();

            var homePageInfo = AppShell.Current.Items.Where(f => f.Route == nameof(HomePage)).FirstOrDefault();
            if (homePageInfo != null) AppShell.Current.Items.Remove(homePageInfo);

            var listDeliveryOrderInfo = AppShell.Current.Items.Where(f => f.Route == nameof(ListDeliveryOrderPage)).FirstOrDefault();
            if (listDeliveryOrderInfo != null) AppShell.Current.Items.Remove(listDeliveryOrderInfo);

            var arrivalDeliveryOrderInfo = AppShell.Current.Items.Where(f => f.Route == nameof(ListArrivalOrderPage)).FirstOrDefault();
            if (arrivalDeliveryOrderInfo != null) AppShell.Current.Items.Remove(arrivalDeliveryOrderInfo);

            var listPutawayPageInfo = AppShell.Current.Items.Where(f => f.Route == nameof(ListPutawayPage)).FirstOrDefault();
            if (listPutawayPageInfo != null) AppShell.Current.Items.Remove(listPutawayPageInfo);

            var listSuccessPutawayInfo = AppShell.Current.Items.Where(f => f.Route == nameof(ListSuccessPutawayPage)).FirstOrDefault();
            if (listSuccessPutawayInfo != null) AppShell.Current.Items.Remove(listSuccessPutawayInfo);

            var listPickOrderPageInfo = AppShell.Current.Items.Where(f => f.Route == nameof(ListPickOrderPage)).FirstOrDefault();
            if (listPickOrderPageInfo != null) AppShell.Current.Items.Remove(listPickOrderPageInfo);

            var listPickPageInfo = AppShell.Current.Items.Where(f => f.Route == nameof(ListCurrentPickPage)).FirstOrDefault();
            if (listPickPageInfo != null) AppShell.Current.Items.Remove(listPickPageInfo);

            var listSuccessPickPageInfo = AppShell.Current.Items.Where(f => f.Route == nameof(ListSuccessPickPage)).FirstOrDefault();
            if (listSuccessPickPageInfo != null) AppShell.Current.Items.Remove(listSuccessPickPageInfo);

            var morePage = AppShell.Current.Items.Where(f => f.Route == nameof(MorePage)).FirstOrDefault();
            if (morePage != null) AppShell.Current.Items.Remove(morePage);


            if (App.UserInfo.ProfileId != 2)
            {
                var flyoutItem = new FlyoutItem()
                {
                    Title = "Home Page",
                    Route = nameof(HomePage),
                    FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems,
                    Items =
                            {
                                new ShellContent
                                {
                                    Icon = Icon.Home,
                                    Title = "Home",
                                    ContentTemplate = new DataTemplate(typeof(HomePage)),
                                },
                                new Tab
                                {
                                    Title="Incoming",
                                    Icon=Icon.Deliveryorder,
                                    Items =
                                    {
                                        new ShellContent
                                        {
                                            Title = "List Delivery order",
                                            ContentTemplate = new DataTemplate(typeof(ListDeliveryOrderPage)),
                                        },
                                        new ShellContent
                                        {
                                            Title="Success Arrival",
                                            ContentTemplate=new DataTemplate(typeof(ListArrivalOrderPage)),
                                        },
                                    }
                                },
                                new Tab
                                {
                                    Title="Put away",
                                    Icon=Icon.Putaway,
                                    Items =
                                    {
                                         new ShellContent
                                        {
                                            Title = "List Putaway",
                                            ContentTemplate = new DataTemplate(typeof(ListPutawayPage)),
                                        },
                                         new ShellContent
                                         {
                                             Title="Success Putaway",
                                             ContentTemplate=new DataTemplate(typeof(ListSuccessPutawayPage))
                                         }
                                    }
                                },

                                   new Tab
                                {
                                    Icon = Icon.Pickorder,
                                    Title = "Outgoing",
                                       Items =
                                       {
                                           new ShellContent
                                           {
                                               Title="List Order",
                                               ContentTemplate = new DataTemplate(typeof(ListPickOrderPage)),
                                           },
                                           new ShellContent
                                           {
                                               Title="List Pick",
                                               ContentTemplate = new DataTemplate(typeof(ListCurrentPickPage)),
                                           },
                                           new ShellContent
                                           {
                                               Title="Success Pick",
                                               ContentTemplate = new DataTemplate(typeof(ListSuccessPickPage)),
                                           }
                                       }
                                },
                                    new ShellContent
                                {
                                    Icon = Icon.More,
                                    Title = "More",
                                    ContentTemplate = new DataTemplate(typeof(MorePage)),
                                },
                            }
                };
                if (!AppShell.Current.Items.Contains(flyoutItem))
                {
                    AppShell.Current.Items.Add(flyoutItem);
                    if (DeviceInfo.Platform == DevicePlatform.WinUI)
                    {
                        AppShell.Current.Dispatcher.Dispatch(async () =>
                        {
                            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                        });
                    }
                    else
                    {
                        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                    }
                }

            }

            if (App.UserInfo.ProfileId == 2)
            {
                var flyoutItem = new FlyoutItem()
                {
                    Title = "Home Page",
                    Route = nameof(HomePage),
                    FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems,
                    Items =
                    {
                                 new ShellContent
                                {
                                    Icon = Icon.Home,
                                    Title = "Home",
                                    ContentTemplate = new DataTemplate(typeof(HomePage)),
                                },
                                new ShellContent
                                {
                                    Icon = Icon.Dashboard,
                                    Title = "Dashboard",
                                    ContentTemplate = new DataTemplate(typeof(MorePage)),
                                },
                                new ShellContent
                                {
                                    Icon = Icon.Deliveryorder,
                                    Title = "Delivery Order",
                                    ContentTemplate = new DataTemplate(typeof(MorePage)),
                                },
                                new ShellContent
                                {
                                    Icon = Icon.Pickorder,
                                    Title = "Sales Order",
                                    ContentTemplate = new DataTemplate(typeof(MorePage)),
                                },
                                 new ShellContent
                                {
                                    Icon = Icon.More,
                                    Title = "More",
                                    ContentTemplate = new DataTemplate(typeof(HomePage)),
                                },
                   }
                };

                if (!AppShell.Current.Items.Contains(flyoutItem))
                {
                    AppShell.Current.Items.Add(flyoutItem);
                    if (DeviceInfo.Platform == DevicePlatform.WinUI)
                    {
                        AppShell.Current.Dispatcher.Dispatch(async () =>
                        {
                            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                        });
                    }
                    else
                    {
                        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                    }
                }
            }

            //if (App.UserDetails.RoleID == (int)RoleDetails.Admin)
            //{
            //    var flyoutItem = new FlyoutItem()
            //    {
            //        Title = "Dashboard Page",
            //        Route = nameof(AdminDashboardPage),
            //        FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems,
            //        Items =
            //        {
            //                    new ShellContent
            //                    {
            //                        Icon = Icons.Dashboard,
            //                        Title = "Admin Dashboard",
            //                        ContentTemplate = new DataTemplate(typeof(AdminDashboardPage)),
            //                    },
            //                    new ShellContent
            //                    {
            //                        Icon = Icons.AboutUs,
            //                        Title = "Admin Profile",
            //                        ContentTemplate = new DataTemplate(typeof(AdminDashboardPage)),
            //                    },
            //       }
            //    };

            //    if (!AppShell.Current.Items.Contains(flyoutItem))
            //    {
            //        AppShell.Current.Items.Add(flyoutItem);
            //        if (DeviceInfo.Platform == DevicePlatform.WinUI)
            //        {
            //            AppShell.Current.Dispatcher.Dispatch(async () =>
            //            {
            //                await Shell.Current.GoToAsync($"//{nameof(AdminDashboardPage)}");
            //            });
            //        }
            //        else
            //        {
            //            await Shell.Current.GoToAsync($"//{nameof(AdminDashboardPage)}");
            //        }
            //    }


            //}
        }

        public static string ValidasiError(HttpStatusCode status)
        {
            if (status == HttpStatusCode.Unauthorized)
            {
                return "Connection Unauthorized";
            }

            if (status == HttpStatusCode.NotFound)
            {
                return "Resource Notfound";
            }

            if (status == HttpStatusCode.RequestTimeout)
            {
                return "Request timeout";
            }

            if (status == HttpStatusCode.InternalServerError)
            {
                return "Internal server error";
            }
            if (status == HttpStatusCode.MethodNotAllowed)
            {
                return "Method not allowed";
            }
            if (status == HttpStatusCode.BadGateway)
            {
                return "Connection bad";
            }

            return null;

        }

    }
}
