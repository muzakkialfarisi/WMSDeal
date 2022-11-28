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
using WMSDeal_uat.Models;

namespace WMSDeal.ViewModels.Pickorder
{
    public class SuccessPickViewModel : BaseViewModel
    {
        private readonly ISalesOrderService salesOrderService = new SalesOrderService();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public virtual ObservableCollection<SalesOrder> SalesOrders { get; set; } = new ObservableCollection<SalesOrder>();
        public virtual ObservableCollection<SelectPeriode> ListPeriode { get; set; } = new ObservableCollection<SelectPeriode>();

        private void GetPeriode()
        {
            for (int i = 0; i < 3; i++)
            {
                SelectPeriode periode = new SelectPeriode();
                periode.Name = DateTime.Now.AddMonths(-i).ToString("MMMM yyyy");
                periode.Value = DateTime.Now.AddMonths(-i);
                ListPeriode.Add(periode);
            }
            SelectedPeriode = ListPeriode[0];
        }
        SelectPeriode _selectPeriode;
        public SelectPeriode SelectedPeriode
        {
            get => _selectPeriode;
            set
            {
                SetProperty(ref _selectPeriode, value);
                Periode = _selectPeriode.Value.ToString("MMyyyy");

                ListSalesOrder(Periode);
            }
        }
        string _periode;
        public string Periode
        {
            get => _periode;
            set
            {
                SetProperty(ref _periode, value);
            }
        }

        public SuccessPickViewModel()
        {
            GetPeriode();
        }

        public ICommand RefreshCommand => new Command(() =>
        {
                ListSalesOrder(Periode);
        });
        public ICommand ScanCommand => new Command(() =>
        {
                ListSalesOrder(Periode);
        });
        private void ListSalesOrder(string Periode)
        {
            IsRefreshing = false;
            IsBusy = true;
            Task.Run(async () =>
            {
                await Task.Delay(500);

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
                            var response = await salesOrderService.GetSalesOrders(App.UserInfo.HouseCode.Trim(), "3,4,5,6", "1",Periode);
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
                toast.Show(cancellationTokenSource.Token);
            }
            });

        }

    }

}
