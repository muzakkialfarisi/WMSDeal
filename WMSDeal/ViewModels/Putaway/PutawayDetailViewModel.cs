

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
using WMSDeal.Services;
using WMSDeal.Views.Startup;

namespace WMSDeal.ViewModels.Putaway
{
    [QueryProperty(nameof(DeliveryOrder), "DeliveryOrder")]
    public partial class PutawayDetailViewModel : BaseViewModel, IRecipient<RefreshCollection>
    {
        private int _qty, _totalPut;
        private Color _color;
        private DeliveryOrder _deliveryOrder;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly IDeliveryOrderService deliveryOrdeService = new DeliveryOrderService();
        private readonly IPutawayService putawayService = new PutawayService();
        public ObservableCollection<ProductData> productData { get; set; } = new ObservableCollection<ProductData>();
        public DeliveryOrder DeliveryOrder
        {
            get => _deliveryOrder;
            set => SetProperty(ref _deliveryOrder, value);
        }

        public int TotalPut
        {
            get => _totalPut;
            set
            {
                SetProperty(ref _totalPut, value);
            }
        }
        public int Qty
        {
            get => _qty;
            set
            {
                SetProperty(ref _qty, value);
            }
        }
        public Color Color
        {
            get => _color;
            set { SetProperty(ref _color, value); }
        }
        public PutawayDetailViewModel()
        {
            //WeakReferenceMessenger.Default.Register<RefreshCollection>(this);

                Waiting();
        }
        public void Waiting()
        {
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
                                var response = await putawayService.GetProductsDeliveryOrder(_deliveryOrder.DoNumber);
                                productData.Clear();

                                if (response.Code == System.Net.HttpStatusCode.OK)
                                {
                                    if (response.Data == "[]")
                                    {
                                        var toast = Toast.Make("Data Notfound!");
                                        await toast.Show(cancellationTokenSource.Token);
                                    }
                                    else
                                    {
                                        List<ProductData> productDataList = JsonConvert.DeserializeObject<List<ProductData>>(response.Data);
                                        TotalPut = 0;
                                        Qty = 0;
                                        foreach (var product in productDataList)
                                        {
                                            if (product.Status != "Arrived")
                                            {
                                                product.Status = "Puted";
                                                product.StatusColor = Color.FromHex("#0149af");
                                                product.Color = Color.FromHex("#e0ffff");
                                                TotalPut++;

                                                // if (product.QtyArrival > 0) Warning = true;
                                            }
                                            else
                                            {
                                                product.Status = "OnProcess";
                                            };

                                            product.BeautyPicture = AppConstant.BaseUrl + "/img/product/" + product.BeautyPicture;
                                            product.isArrival = product.QtyArrival > 0 && product.Status == "OnProcess" ? true : false;
                                            Qty = Qty + product.Quantity;
                                            Color = Color.FromRgb(114, 172, 241);
                                            product.Tenant = _deliveryOrder.Name;
                                            productData.Add(product);

                                        }
                                        if (productData.Count == TotalPut)
                                        {
                                            //WeakReferenceMessenger.Default.Send(new RefreshCollection("RefreshPutaway"));

                                            await Shell.Current.GoToAsync("..");
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

        public void Receive(RefreshCollection message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (message.Value == "RefreshCollection")
                {
                        Waiting();
                }
            });
        }

        public ICommand SelectItemCommand => new Command<ProductData>((Details) =>
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    var ProductData = new Dictionary<string, object>
                        {
                            {"ProductData", Details }
                        };

                    if (Details.DONumber != null)
                    {
                        if (Details.Status == "OnProcess")
                        {
                            //if (Details.ProductLevel == "SKU")
                                Shell.Current.GoToAsync("PutawayProduct", ProductData);
                            //else
                                //Shell.Current.GoToAsync("PutawayProductItem", ProductData);
                        }
                        else
                        {
                            var toast = Toast.Make("Product has been arrived");
                            toast.Show(cancellationTokenSource.Token);
                        }

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
