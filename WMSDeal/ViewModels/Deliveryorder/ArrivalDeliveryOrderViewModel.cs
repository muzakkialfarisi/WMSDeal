using CommunityToolkit.Maui.Alerts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WMSDeal.Constant;
using WMSDeal.Models;
using WMSDeal.Models.Incoming;
using WMSDeal.Services;
using WMSDeal.Views.Deliveryorder;
using WMSDeal.Views.Startup;
using WMSDeal_uat.Models;

namespace WMSDeal.ViewModels.Deliveryorder
{
    public partial class ArrivalDeliveryOrderViewModel : BaseViewModel
    {
        private int _totalar;

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly IDeliveryOrderService deliveryOrderService = new DeliveryOrderService();
        public ObservableCollection<DeliveryOrder> SuccessDeliveryOrders { get; set; } = new ObservableCollection<DeliveryOrder>();
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

                Waiting(Periode);
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
        public int TotalAR
        {
            get => _totalar;
            set
            {
                SetProperty(ref _totalar, value);
                OnPropertyChanged();
            }
        }

        public ArrivalDeliveryOrderViewModel()
        {
            GetPeriode();
        }

        public ICommand RefreshCommand => new Command(() =>
        {
                Waiting(Periode);
        });

        public ICommand ScanCommand => new Command(() =>
        {
                Waiting(Periode);
        });

        private void Waiting(string Periode)
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
                            var response = await deliveryOrderService.GetDeliveryOrders("AR,PUT",Periode);
                            SuccessDeliveryOrders.Clear();

                            if (response.Code == System.Net.HttpStatusCode.OK)
                            {
                                if (response.Data == "[]")
                                {
                                    var toast = Toast.Make("Data Notfound!");
                                    await toast.Show(cancellationTokenSource.Token);
                                }
                                else
                                {
                                    List<DeliveryOrder> deliveryOrdersList = JsonConvert.DeserializeObject<List<DeliveryOrder>>(response.Data);
                                    TotalAR = 0;
                                    foreach (var deliveryOrder in deliveryOrdersList)
                                    {
                                        deliveryOrder.ProfileImageUrl = AppConstant.BaseUrl + "/img/tenant/" + deliveryOrder.ProfileImageUrl;
                                        TotalAR++;
                                        SuccessDeliveryOrders.Add(deliveryOrder);
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


        public ICommand SelectItemCommand => new Command<DeliveryOrder>(async (doDetail) =>
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    var DeliveryOrder = new Dictionary<string, object>
                        {
                            {"DeliveryOrder", doDetail }
                        };

                    if (doDetail.DoNumber != null)
                    {
                        await Shell.Current.GoToAsync(nameof(DeliveryOrderDetailPage), DeliveryOrder);
                    }
                }
                catch (Exception msg)
                {
                    var toast = Toast.Make("Error Exception: " + msg);
                    await toast.Show(cancellationTokenSource.Token);
                }
            }
            else
            {
                var toast = Toast.Make("Connection Lost...");
                toast.Show(cancellationTokenSource.Token);
            }
        });


    }
}
