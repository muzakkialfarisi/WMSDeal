
using CommunityToolkit.Maui.Alerts;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using WMSDeal.Constant;
using WMSDeal.Models;
using WMSDeal.Models.Incoming;
using WMSDeal.Services;
using WMSDeal.Views.Startup;

namespace WMSDeal.ViewModels.Putaway
{
    public partial class PutawayProductItemViewModel : BaseViewModel
    {
        private ProductData _productData;
        private int _totalar, _totalRemaining;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly IDeliveryOrderService deliveryOrderService = new DeliveryOrderService();
        public ObservableCollection<ItemProduct> itemProductModel { get; set; } = new ObservableCollection<ItemProduct>();
        //public ObservableCollection<ArrivalProductModel> ArrivalProduct { get; set; } = new ObservableCollection<ArrivalProductModel>();
        public ProductData ProductData
        {
            get => _productData;
            set
            {
                SetProperty(ref _productData, value);
                OnPropertyChanged();
            }
        }

        public int TotalAr
        {
            get => _totalar;
            set => SetProperty(ref _totalar, value);
        }
        public int TotalRemaining
        {
            get => _totalRemaining;
            set => SetProperty(ref _totalRemaining, value);
        }

        public PutawayProductItemViewModel()
        {
            //IsBusy = true;
            Task.Run(async () =>
            {
                await Task.Delay(200);
                Waiting();

            });
        }

        private void Waiting()
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                Application.Current.Dispatcher.Dispatch(() =>
                {
                    try
                    {
                        itemProductModel.Clear();
                        List<ItemProduct> itemProducts = ProductData.ItemProduct;
                        TotalAr = 0;
                        TotalRemaining = 0;
                        foreach (var itemproduct in itemProducts)
                        {
                            if (itemproduct.Status == "2") TotalRemaining++; else TotalAr++;
                            itemProductModel.Add(itemproduct);
                        }

                    }
                    catch (Exception msg)
                    {
                        var toast = Toast.Make("Error Exception: " + msg);
                         toast.Show(cancellationTokenSource.Token);
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

        private void GetTotalArrival()
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
                            var response = await deliveryOrderService.GetTotalArrival(_productData.DOProductId, _productData.ProductLevel);
                            //ArrivalProduct.Clear();

                            if (response.Code == System.Net.HttpStatusCode.OK)
                            {
                                if (response.Data == "[]")
                                {
                                    var arrival = new ArrivalProduct();
                                    arrival.DOProductId = _productData.DOProductId;
                                    arrival.Quantity = 0;
                                    TotalRemaining = 1;
                                }
                                else
                                {
                                    //if (_productData.ProductLevel == "SKU")
                                    //{
                                    List<ArrivalProduct> arrivalProduct = JsonConvert.DeserializeObject<List<ArrivalProduct>>(response.Data);
                                    foreach (var product in arrivalProduct)
                                    {
                                        TotalAr = product.Quantity;
                                        TotalRemaining = 1;
                                        //DeliveryOrders.Add(deliveryOrder);
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
                    //IsBusy = false;
                });
            }
            else
            {
                var toast = Toast.Make("Connection Lost...");
                toast.Show(cancellationTokenSource.Token);
            }
        }

    }
}
