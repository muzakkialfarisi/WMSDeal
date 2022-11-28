using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
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
using WMSDeal.Messages;
using WMSDeal.Models;
using WMSDeal.Models.Incoming;
using WMSDeal.Services;
using WMSDeal.Views.Deliveryorder;
using WMSDeal.Views.Startup;

namespace WMSDeal.ViewModels.Deliveryorder
{
    public partial class DeliveryOrderViewModel : BaseViewModel, IRecipient<RefreshCollection>
    {
        private int _total = 0;

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly IDeliveryOrderService deliveryOrderService = new DeliveryOrderService();
        public ObservableCollection<DeliveryOrder> DeliveryOrders { get; set; } = new ObservableCollection<DeliveryOrder>();

        public int Total
        {
            get => _total;
            set
            {
                SetProperty(ref _total, value);
                OnPropertyChanged();
            }
        }

        public DeliveryOrderViewModel()
        {
            //WeakReferenceMessenger.Default.Register<RefreshCollection>(this);

                Waiting();
        }

        public ICommand RefreshCommand => new Command(() =>
        {
                Waiting();
        });

        //public ICommand ScanCommand => new Command(() =>
        //{
        //    IsRefreshing = false;
        //    IsBusy = true;
        //    Task.Run(async () =>
        //    {
        //        await Task.Delay(500);
        //        Waiting();

        //    });
        //});

        private void Waiting()
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
                                var response = await deliveryOrderService.GetDeliveryOrders("DO");
                                DeliveryOrders.Clear();

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
                                        Total = 0;
                                        foreach (var deliveryOrder in deliveryOrdersList)
                                        {
                                            deliveryOrder.ProfileImageUrl = AppConstant.BaseUrl + "/img/tenant/" + deliveryOrder.ProfileImageUrl;
                                            Total++;
                                            DeliveryOrders.Add(deliveryOrder);
                                        }

                                        var toast = Toast.Make("Double Tap memilih list!",ToastDuration.Long);
                                        await toast.Show(cancellationTokenSource.Token);

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

        public void Receive(RefreshCollection message)
        {
            if (message.Value == "RefreshOrder")
            {
                Waiting();
            }
        }

        public ICommand SelectItemCommand => new Command<DeliveryOrder>((doDetail) =>
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
                        Shell.Current.GoToAsync(nameof(DeliveryOrderDetailPage), DeliveryOrder);
                    }

                }
                catch (Exception msg)
                {
                    var toast = Toast.Make("Error Exception: " + msg);
                    toast.Show(cancellationTokenSource.Token);
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
