using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Windows.Input;
using WMSDeal.Constant;
using WMSDeal.Messages;
using WMSDeal.Models;
using WMSDeal.Models.Incoming;
using WMSDeal.Models.Inventory;
using WMSDeal.Services;
using WMSDeal.Views.Startup;

namespace WMSDeal.ViewModels.Putaway
{
    [QueryProperty(nameof(ProductData), "ProductData")]
    public partial class PutawayProductViewModel : BaseViewModel, IRecipient<ScanMessage>
    {
        private ProductData _productData;
        private int _totalPut, _totalRemaining;
        private string _total;
        private bool _isVisible = false;
        private string _storage, _storageName, _houseName, _zoneName, _sizeName, _row = "";
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly IPutawayService putawayService = new PutawayService();

        public ObservableCollection<Storage> StorageCode = new ObservableCollection<Storage>();
        public ProductData ProductData
        {
            get => _productData;
            set
            {
                SetProperty(ref _productData, value);
                OnPropertyChanged();
            }
        }

        public int TotalPut
        {
            get => _totalPut;
            set { SetProperty(ref _totalPut, value); }
        }
        public int TotalRemaining
        {
            get => _totalRemaining;
            set { SetProperty(ref _totalRemaining, value); OnPropertyChanged(); }
        }

        public string Total
        {
            get => _total;
            set
            {
                SetProperty(ref _total, value);
                OnPropertyChanged();
            }
        }
        public string Storage
        {
            get => _storage;
            set
            {
                SetProperty(ref _storage, value.ToUpper());
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
        public bool IsVisible
        {
            get => _isVisible;
            set { SetProperty(ref _isVisible, value); }
        }
        public PutawayProductViewModel()
        {
            WeakReferenceMessenger.Default.Register<ScanMessage>(this);

            Task.Run(async () =>
            {
                await Task.Delay(200);
                GetTotalPuted();
            });
        }

        public void Receive(ScanMessage message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                //Shell.Current.DisplayAlert("Alert", message.Value, "OK");

                //Storage = message.Value;



                if (message.Value != "")
                {
                    //    IsBusy = true;
                    //Task.Run(async () =>
                    //{
                    //    await Task.Delay(200);

                    GetStorageCode(message.Value);
                    //});
                }
            });
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
                                if (response.Data == "")
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
                            else if (Total == null || Total == "0")
                            {
                                var toast = Toast.Make("Quantity put required");
                                await toast.Show(cancellationTokenSource.Token);
                            }
                            else
                            {
                                if (Int32.Parse(Total) > TotalRemaining)
                                {
                                    var toast = Toast.Make("Quantity melebihi remaining");
                                    await toast.Show(cancellationTokenSource.Token);
                                }
                                else if (Int32.Parse(Total) < 0)
                                {
                                    var toast = Toast.Make("Quantity  invalid");
                                    await toast.Show(cancellationTokenSource.Token);
                                }
                                else
                                {
                                    var model = new PutawayModel();
                                    model.DOProductId = ProductData.DOProductId;
                                    model.IKU = "";
                                    model.Quantity = Int32.Parse(Total);
                                    model.StorageCode = new Guid(Storage);

                                    var response = await putawayService.UpdatePutaway(model);
                                    if (response.Code == System.Net.HttpStatusCode.OK)
                                    {
                                        var toast = Toast.Make(response.Data.ToString());
                                        await toast.Show(cancellationTokenSource.Token);

                                        //WeakReferenceMessenger.Default.Send(new RefreshCollection("RefreshCollection"));

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
                        }
                        catch (Exception msg)
                        {
                            var toast = Toast.Make("Error Exception " + msg);
                            await toast.Show(cancellationTokenSource.Token);
                        }
                        IsBusy = false;
                    });
                }
               
            }
            else
            {
                var toast = Toast.Make("Connection Lost...");
                await toast.Show(cancellationTokenSource.Token);
            }

        });
        private async void GetTotalPuted()
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
                            var response = await putawayService.GetTotalPuted(_productData.DOProductId);
                            if (response.Code == System.Net.HttpStatusCode.OK)
                            {
                                if (response.Data == "[]")
                                {
                                    var putaway = new ArrivalProduct();
                                    putaway.DOProductId = _productData.DOProductId;
                                    putaway.Quantity = 0;
                                    TotalRemaining = ProductData.Quantity;
                                }
                                else
                                {
                                    List<ArrivalProduct> putawayProduct = JsonConvert.DeserializeObject<List<ArrivalProduct>>(response.Data);
                                    foreach (var product in putawayProduct)
                                    {
                                        TotalPut += product.Quantity;
                                    }
                                    TotalRemaining = ProductData.Quantity - TotalPut;
                                }
                            }
                        }
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
                var toast = Toast.Make("Connection Lost..");
                await toast.Show(cancellationTokenSource.Token);
            }
        }
    }
}
