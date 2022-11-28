using CommunityToolkit.Maui.Alerts;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Windows.Input;
using WMSDeal.Constant;
using WMSDeal.Models;
using WMSDeal.Models.Outgoing;
using WMSDeal.Services;
using WMSDeal.Views.Startup;

namespace WMSDeal.ViewModels.Pickorder
{
    public partial class PickOrderViewModel : BaseViewModel
    {
        private readonly ISalesOrderService salesOrderService = new SalesOrderService();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public virtual ObservableCollection<SalesOrder> SalesOrders { get; set; } = new ObservableCollection<SalesOrder>();

        public List<SalesOrderAssign> SalesOrderAssignList = new List<SalesOrderAssign>();

        private int _totalSelected=0;

        public PickOrderViewModel()
        {
            ListSalesOrder();
        }

        public int TotalSelected
        {
            get => _totalSelected;
            set
            {
                SetProperty(ref _totalSelected, value);
                OnPropertyChanged();
            }
        }
        public ICommand RefreshCommand => new Command(() =>
        {
            if (TotalSelected == 0)
            {
                TotalSelected = 0;
                ListSalesOrder();
            }
            else
            {
                var toast = Toast.Make("Ada list yang di pilih");
                toast.Show(cancellationTokenSource.Token);
            }
        });
        public ICommand ScanCommand => new Command(() =>
        {
            ListSalesOrder();
        });
        private void ListSalesOrder()
        {
            IsRefreshing = false;
            IsBusy = true;
            Task.Run(async () =>
            {
                await Task.Delay(200);
                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    Application.Current.Dispatcher.Dispatch(async () =>
                    {
                        try
                        {
                            var tokenDetails = await SecureStorage.GetAsync(nameof(App.Token));
                            var jsonToken = new JwtSecurityTokenHandler().ReadToken(tokenDetails) as JwtSecurityToken;

                            if (jsonToken.ValidTo < DateTime.UtcNow)
                            {
                                await Shell.Current.DisplayAlert("Deal", "Sesi Expired. Login lagi untuk melanjutkan", "OK");
                                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                            }
                            else
                            {
                                var response = await salesOrderService.GetSalesOrders(App.UserInfo.HouseCode.Trim(), "2", "0");
                                SalesOrders.Clear();

                                if (response.Code == System.Net.HttpStatusCode.OK)
                                {
                                    if (response.Data == "[]")
                                    {
                                        var toast = Toast.Make("Data Notfound!");
                                        await toast.Show(cancellationTokenSource.Token);
                                    }
                                    else
                                    {
                                        List<SalesOrder> salesOrderList = JsonConvert.DeserializeObject<List<SalesOrder>>(response.Data);
                                        foreach (var salesOrder in salesOrderList)
                                        {
                                            salesOrder.ImageTenant = AppConstant.BaseUrl + "/img/tenant/" + salesOrder.ImageTenant;
                                            SalesOrders.Add(salesOrder);
                                        }
                                    }
                                }
                                else
                                {
                                    if (response.Data == "")
                                    {
                                        var pesan = AppConstant.ValidasiError(response.Code);
                                        if (pesan != null)
                                        {
                                            var toast = Toast.Make(pesan);
                                            await toast.Show(cancellationTokenSource.Token);
                                        }
                                    }
                                    else
                                    {
                                        var _error = JsonConvert.DeserializeObject<ErrorResponse>(response.Data.ToString());
                                        if (_error != null)
                                        {
                                            var toast = Toast.Make(_error.Message);
                                            await toast.Show(cancellationTokenSource.Token);
                                        }
                                    }
                                }
                            }

                        }
                        catch (Exception msg)
                        {
                            var toast = Toast.Make("Error Exception: " + msg);
                            await toast.Show(cancellationTokenSource.Token);
                        }
                        IsBusy = false;
                    });
                }
                else
                {
                    var toast = Toast.Make("Connection Lost...");
                   await toast.Show(cancellationTokenSource.Token);
                }
            });


        }

        public ICommand AssignSelectedCommand => new Command(() =>
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                Application.Current.Dispatcher.Dispatch(async () =>
                {
                    try
                    {
                        IsBusy = true;
                        await Task.Delay(100);

                        var tokenDetails = await SecureStorage.GetAsync(nameof(App.Token));
                        var jsonToken = new JwtSecurityTokenHandler().ReadToken(tokenDetails) as JwtSecurityToken;

                        if (jsonToken.ValidTo < DateTime.UtcNow)
                        {
                            await Shell.Current.DisplayAlert("Deal", "Sesi Expired. Login lagi untuk melanjutkan", "OK");
                            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                        }
                        else
                        {
                            //Assign

                            var response = await salesOrderService.AssignSalesOrder(App.UserInfo.HouseCode, SalesOrderAssignList);

                            if (response.Code == System.Net.HttpStatusCode.OK)
                            {
                                var toast = Toast.Make(response.Data.ToString());
                                await toast.Show(cancellationTokenSource.Token);

                                TotalSelected = 0;

                                ListSalesOrder();
                            }
                            else
                            {
                                if (response.Data == "")
                                {
                                    var pesan = AppConstant.ValidasiError(response.Code);
                                    if (pesan != null)
                                    {
                                        var toast = Toast.Make(pesan);
                                        await toast.Show(cancellationTokenSource.Token);
                                    }
                                }
                                else
                                {
                                    var _error = JsonConvert.DeserializeObject<ErrorResponse>(response.Data.ToString());
                                    if (_error != null)
                                    {
                                        var toast = Toast.Make(_error.Message);
                                        await toast.Show(cancellationTokenSource.Token);
                                    }
                                }
                            }
                        }

                        IsBusy = false;
                    }
                    catch (Exception msg)
                    {
                        var toast = Toast.Make("Error Exception: " + msg);
                        await toast.Show(cancellationTokenSource.Token);
                    }
                });
            }
            else
            {
                var toast = Toast.Make("Connection Lost...");
                toast.Show(cancellationTokenSource.Token);
            }
        });
        public ICommand SelectItemCommand => new Command<IList<object>>(async (salesOrder) =>
        {
            //var selectedItem = salesOrder as SalesOrderModel;
            // List<SalesOrderModel> salesOrderModels = new List<SalesOrderModel>();

            SalesOrderAssignList.Clear();

            TotalSelected = 0;
            foreach (var item in salesOrder)
            {
                TotalSelected++;
                var selectedItems = item as SalesOrder;

                var order = new SalesOrderAssign();
                order.OrderId = selectedItems.OrderId;

                SalesOrderAssignList.Add(order);

                //salesOrderModels.Add(selectedItems);
            }
        });
    }
}
