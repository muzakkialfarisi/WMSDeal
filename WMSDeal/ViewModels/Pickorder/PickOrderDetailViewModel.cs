using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Windows.Input;
using WMSDeal.Constant;
using WMSDeal.Messages;
using WMSDeal.Models;
using WMSDeal.Models.Inventory;
using WMSDeal.Models.Outgoing;
using WMSDeal.Services;
using WMSDeal.Views.Startup;

namespace WMSDeal.ViewModels.Pickorder
{
    [QueryProperty(nameof(SalesOrderPick), "Pick")]
    public partial class PickOrderDetailViewModel : BaseViewModel, IRecipient<ScanMessage>
    {
        SalesOrderPick _pickOrder;
        private readonly IPutawayService putawayService = new PutawayService();
        private readonly ISalesOrderService salesOrderService = new SalesOrderService();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public ObservableCollection<Storage> StorageCode = new ObservableCollection<Storage>();
        private string _storageName, _houseName, _zoneName, _sizeName, _row = "";
        public SalesOrderPick SalesOrderPick
        {
            get => _pickOrder;
            set => SetProperty(ref _pickOrder, value);
        }

        private string _saveText = "SAVE";
        public string SaveText
        {
            get => _saveText;
            set => SetProperty(ref _saveText, value);
        }
        bool _isVisible = false;
        public bool IsVisible
        {
            get => _isVisible;
            set { SetProperty(ref _isVisible, value); }
        }

        string _storage;
        public string Storage
        {
            get => _storage;
            set
            {
                SetProperty(ref _storage, value);
                OnPropertyChanged();
            }
        }
        public string StorageName
        {
            get => _storageName;
            set
            {
                SetProperty(ref _storageName, value);
            }
        }

        public string HouseName
        {
            get => _houseName;
            set
            {
                SetProperty(ref _houseName, value);
            }
        }

        public string ZoneName
        {
            get => _zoneName;
            set
            {
                SetProperty(ref _zoneName, value);
            }
        }

        public string SizeName
        {
            get => _sizeName;
            set
            {
                SetProperty(ref _sizeName, value);
            }
        }

        public string Row
        {
            get => _row;
            set { SetProperty(ref _row, value); }
        }

        public PickOrderDetailViewModel()
        {
            WeakReferenceMessenger.Default.Register<ScanMessage>(this);
        }
        private async void GetStorageCode(string storage)
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                Application.Current.Dispatcher.Dispatch(async () =>
                {
                    try
                    {
                        var tokenDetails = await SecureStorage.GetAsync(nameof(App.Token));
                        var jsonToken = new JwtSecurityTokenHandler().ReadToken(tokenDetails) as JwtSecurityToken;

                        if (jsonToken.ValidTo < DateTime.Now)
                        {
                            await Shell.Current.DisplayAlert("Deal", "Sesi Expired. Login lagi untuk melanjutkan", "OK");
                            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                        }
                        else
                        {
                            var response = await putawayService.GetStorageCode(storage);
                            if (response.Code == System.Net.HttpStatusCode.OK)
                            {
                                if (response.Data == "[]")
                                {
                                    var toast = Toast.Make("Storage Code Notfound");
                                    await toast.Show(cancellationTokenSource.Token);
                                }
                                else
                                {
                                    StorageCode.Clear();
                                    List<Storage> storageCodes = JsonConvert.DeserializeObject<List<Storage>>(response.Data);

                                    foreach (var storageCode in storageCodes)
                                    {
                                        if (storageCode.HouseCode == App.UserInfo.HouseCode)
                                        {
                                            Storage = storageCode.StorageCode;

                                            string rowCode = storageCode.RowCode.Substring((storageCode.HouseCode + storageCode.ZoneCode).Length);
                                            string levelCode = storageCode.LevelCode.Substring(storageCode.RowCode.Length);
                                            string binCode = storageCode.BinCode.Substring(storageCode.LevelCode.Length);

                                            StorageName = storageCode.HouseCode + "-" + storageCode.ZoneCode + "-" + rowCode + "-" + levelCode + "/" + binCode;
                                            HouseName = storageCode.HouseName;
                                            ZoneName = storageCode.ZoneName;
                                            SizeName = storageCode.SizeName;
                                            Row = rowCode;
                                            IsVisible = true;

                                            StorageCode.Add(storageCode);
                                        }
                                        else
                                        {
                                            IsVisible = false;
                                            var toast = Toast.Make("Storage Code have " + storageCode.HouseName);
                                            await toast.Show(cancellationTokenSource.Token);
                                        }
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
                        StorageName = "";
                        HouseName = "";
                        ZoneName = "";
                        SizeName = "";
                        Row = "";
                        IsVisible = false;

                        var toast = Toast.Make("Error exception : " + msg.Message);
                        await toast.Show(cancellationTokenSource.Token);
                    }
                });
            }
            else
            {
                var toast = Toast.Make("Connection Lost...");
                await toast.Show(cancellationTokenSource.Token);
            }
        }

        public void Receive(ScanMessage message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (message.Value != "")
                {
                    GetStorageCode(message.Value);
                }
            });
        }

        public ICommand ScanCommand => new Command(async () =>
        {
            await Shell.Current.GoToAsync("ScanBarcode");
        });

        public ICommand SimpanCommand => new Command(async () =>
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                if (Storage == "" || Storage == null)
                {
                    var toast = Toast.Make("Storage Code not found!");
                    await toast.Show(cancellationTokenSource.Token);
                }
                else
                {
                    Application.Current.Dispatcher.Dispatch(async () =>
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
                            else if (SalesOrderPick.Id == 0 || Storage == null)
                            {
                                var toast = Toast.Make("Id Pick atau Storage code required");
                                await toast.Show(cancellationTokenSource.Token);
                            }
                            else
                            {
                                var model = new PickOrder();
                                model.Id = SalesOrderPick.Id;
                                model.StorageCode = new Guid(Storage);

                                var response = await salesOrderService.SavePick(model);
                                if (response.Code == System.Net.HttpStatusCode.OK)
                                {
                                    var toast = Toast.Make(response.Data.ToString());
                                    await toast.Show(cancellationTokenSource.Token);

                                    WeakReferenceMessenger.Default.Send(new RefreshCollection("RefreshCollection"));

                                    await Shell.Current.GoToAsync("..");
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
                            var toast = Toast.Make("Error exception:" + ex.Message);
                            await toast.Show(cancellationTokenSource.Token);
                        }
                        IsBusy = false;
                    });
                }
            }
            else
            {
                var toast = Toast.Make("Lost Connection...");
                await toast.Show(cancellationTokenSource.Token);
            }

        });



        public ICommand ShowLoadCommand => new Command(() =>
        {
            IsBusy = true;
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                IsBusy = false;
            });
        });

    }
}
