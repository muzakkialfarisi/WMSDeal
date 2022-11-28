using CommunityToolkit.Maui.Alerts;
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
using WMSDeal.Models.Outgoing;
using WMSDeal.Services;
using WMSDeal.Views.Pickorder;
using WMSDeal.Views.Startup;

namespace WMSDeal.ViewModels.Pickorder
{
    [QueryProperty(nameof(Nama),"nama")]
    public partial class PickViewModel : BaseViewModel, IRecipient<RefreshCollection>
    {
        private readonly ISalesOrderService salesOrderService = new SalesOrderService();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public virtual ObservableCollection<SalesOrderPick> SalesOrderPicks { get; set; } = new ObservableCollection<SalesOrderPick>();

        int _totalPicked = 0;
        string PickAssignId = "";

        string _nama;
        public string Nama
        {
            get => _nama;
            set
            {
                SetProperty(ref _nama, value);
                OnPropertyChanged();
            }
        }
        bool isStaging = false;
        public bool IsStaging
        {
            get => isStaging;
            set
            {
                SetProperty(ref isStaging, value);
                OnPropertyChanged();
            }
        }
        public int TotalPicked
        {
            get => _totalPicked;
            set
            {
                SetProperty(ref _totalPicked, value);
                OnPropertyChanged();
            }
        }
        public ICommand Page2Command => new Command(async () =>
        {
            string param = "Usman";
            await Shell.Current.GoToAsync($"PickOrderDetailPage?nama={param}");
        });

        public ICommand ScanCommand => new Command(() =>
        {
            IsBusy = true;
            Task.Run(async () =>
            {
                await Task.Delay(200);
                ListPickSalesOrder();
            });
        });

        public ICommand RefreshCommand => new Command(() =>
        {
            IsRefreshing = false;
            IsBusy = true;
            Task.Run(async () =>
            {
                await Task.Delay(200);
                ListPickSalesOrder();
            });
        });
        public PickViewModel()
        {
            WeakReferenceMessenger.Default.Register<RefreshCollection>(this);

            IsRefreshing = false;
            IsBusy = true;
            Task.Run(async () =>
            {
                await Task.Delay(200);
                ListPickSalesOrder();
            });
        }

        private void ListPickSalesOrder()
        {
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
                            var response = await salesOrderService.GetSalesOrderPick();
                            SalesOrderPicks.Clear();

                            if (response.Code == System.Net.HttpStatusCode.OK)
                            {
                                if (response.Data == "[]")
                                {
                                    var toast = Toast.Make("Data Notfound!");
                                    await toast.Show(cancellationTokenSource.Token);
                                    TotalPicked = 0;
                                    IsStaging = false;
                                }
                                else
                                {
                                    TotalPicked = 0;
                                    List<SalesOrderPick> salesOrderList = JsonConvert.DeserializeObject<List<SalesOrderPick>>(response.Data);
                                    foreach (var salesOrder in salesOrderList)
                                    {
                                        PickAssignId = salesOrder.PickAssignId;
                                        if (salesOrder.PickedStatus == "Picked")
                                        {
                                            salesOrder.StatusColor = Color.FromHex("#235d3a");
                                            TotalPicked++;
                                        }
                                        salesOrder.BeautyPicture = AppConstant.BaseUrl + "/img/product/" + salesOrder.BeautyPicture;
                                        SalesOrderPicks.Add(salesOrder);
                                    }
                                    if (TotalPicked == salesOrderList.Count && salesOrderList.Count > 0)
                                    {
                                        IsStaging = true;

                                        // SaveStaging();
                                    }
                                    else { IsStaging = false; }
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
        }

        public ICommand SelectItemCommand => new Command<SalesOrderPick>(async (pick) =>
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    var pickOrder = new Dictionary<string, object>
                        {
                            {"Pick", pick }
                        };

                    if (pick.Id != 0)
                    {
                        if (pick.PickedStatus == "Ordered")
                        {
                            await Shell.Current.GoToAsync(nameof(PickOrderDetailPage), pickOrder);
                        }
                        else
                        {
                            var toast = Toast.Make("Product sudah di pick");
                            await toast.Show(cancellationTokenSource.Token);
                        }
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
                await toast.Show(cancellationTokenSource.Token);
            }
        });

        public ICommand StagingCommand => new Command(async () =>
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                Application.Current.Dispatcher.Dispatch(async () =>
                {
                    SaveStaging();
                });
            }
            else
            {
                var toast = Toast.Make("Lost Connection...");
                await toast.Show(cancellationTokenSource.Token);
            }
        });

        public async void SaveStaging()
        {
            try
            {
                IsBusy = true;
                await Task.Delay(100);
                var token = await SecureStorage.GetAsync(nameof(App.Token));
                var jsonToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
                if (jsonToken.ValidTo < DateTime.UtcNow)
                {
                    await Shell.Current.DisplayAlert("Deal", "Sesi Expired. Login lagi untuk melanjutkan", "OK");
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
                else
                {
                    var model = new Staging();
                    model.UserId = App.UserInfo.UserId.ToString();
                    model.PickAssignId = PickAssignId;

                    var response = await salesOrderService.Staging(model);
                    if (response.Code == System.Net.HttpStatusCode.OK)
                    {
                        var toast = Toast.Make(response.Data.ToString());
                        await toast.Show(cancellationTokenSource.Token);

                        SalesOrderPicks.Clear();
                        TotalPicked = 0;
                        ListPickSalesOrder();
                        // WeakReferenceMessenger.Default.Send(new RefreshCollection("Staging"));

                        //await Shell.Current.GoToAsync("..");
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
            catch (Exception ex)
            {
                var toast = Toast.Make("Error Exception: " + ex.Message);
                await toast.Show(cancellationTokenSource.Token);
            }
            IsBusy = false;
        }

        public void Receive(RefreshCollection message)
        {
            if (message.Value == "RefreshCollection")
            {
                IsRefreshing = false;
                IsBusy = true;
                Task.Run(async () =>
                {
                    await Task.Delay(200);
                    ListPickSalesOrder();
                });
            }
        }
    }
}
