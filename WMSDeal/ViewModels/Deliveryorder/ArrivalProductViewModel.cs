

#if __ANDROID__
using Android.Graphics;
#endif

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Windows.Input;
using WMSDeal.Constant;
using WMSDeal.Messages;
using WMSDeal.Models;
using WMSDeal.Models.Incoming;
using WMSDeal.Services;
using WMSDeal.Views.Startup;

namespace WMSDeal.ViewModels.Deliveryorder
{
    [QueryProperty(nameof(ProductData), "ProductData")]
    public partial class ArrivalProductViewModel : BaseViewModel
    {
        string _photobase64, _note = "";
        bool _showphoto;
        string _total = "";
        string _photopath = Icon.Camera;

        private ProductData _productData;
        private int _totalar, _totalRemaining;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly IDeliveryOrderService deliveryOrdeService = new DeliveryOrderService();
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
        public string Note
        {
            get => _note;
            set
            {
                SetProperty(ref _note, value);
                OnPropertyChanged();
            }
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

        public Command CapturePhotoCommand { get; }
        public string PhotoPath
        {
            get => _photopath;
            set => SetProperty(ref _photopath, value);
        }

        public bool ShowPhoto
        {
            get => _showphoto;
            set => SetProperty(ref _showphoto, value);
        }

        public string Photobase64
        {
            get => _photobase64;
            set
            {
                SetProperty(ref _photobase64, value);
                OnPropertyChanged();
            }
        }
        private string _saveText = "SAVE";
        public string SaveText
        {
            get => _saveText;
            set => SetProperty(ref _saveText, value);
        }

        public ArrivalProductViewModel()
        {
            //IsBusy = true;
            ShowPhoto = true;
            Task.Run(async () =>
            {
                await Task.Delay(200);
                GetTotalArrival();

            });

            CapturePhotoCommand = new Command(TakePhoto);
        }

        public async void TakePhoto()
        {
            if (MediaPicker.IsCaptureSupported)
            {
                FileResult photo = await MediaPicker.CapturePhotoAsync();
                if (photo != null)
                {
                    //Load to View
                    await LoadPhotoAsync(photo);

                }
            }
        }

        async Task LoadPhotoAsync(FileResult photo)
        {
            // canceled
            try
            {
                if (photo == null)
                {
                    PhotoPath = null;
                    return;
                }

                // save the file into local storage
                var newFile = System.IO.Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                using (var stream = await photo.OpenReadAsync())
                using (var newStream = File.OpenWrite(newFile))
                {
                    await stream.CopyToAsync(newStream);
                }

#if __ANDROID__
                byte[] imageArray = File.ReadAllBytes(newFile);

                byte[] resizeImage;
                resizeImage =ResizeImageAndroid(imageArray, 366, 390);

                string base64String = Convert.ToBase64String(resizeImage);
                PhotoPath = newFile;

                Photobase64 = base64String;

                ShowPhoto = true;
#endif

                //#if __IOS__
                //                byte[] resizeImage;
                //                resizeImage = ResizeImageIOS(imageArray, 366, 390);
                //                string base64String = Convert.ToBase64String(resizeImage);
                //                PhotoPath = newFile;

                //                Photobase64 = base64String;

                //                ShowPhoto = true;
                //#endif


            }
            catch (Exception msg)
            {
                Shell.Current.DisplayAlert("alert", msg.Message, "OK");
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
                            var response = await deliveryOrdeService.GetTotalArrival(_productData.DOProductId, _productData.ProductLevel);
                            //ArrivalProduct.Clear();

                            if (response.Code == System.Net.HttpStatusCode.OK)
                            {
                                if (response.Data == "[]")
                                {
                                    var arrival = new ArrivalProduct();
                                    arrival.DOProductId = _productData.DOProductId;
                                    arrival.Quantity = 0;
                                    TotalRemaining = ProductData.Quantity;
                                }
                                else
                                {
                                    //if (_productData.ProductLevel == "SKU")
                                    //{
                                    List<ArrivalProduct> arrivalProduct = JsonConvert.DeserializeObject<List<ArrivalProduct>>(response.Data);
                                    foreach (var product in arrivalProduct)
                                    {
                                        TotalAr = product.Quantity;
                                        TotalRemaining = ProductData.Quantity - TotalAr;
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

        public ICommand SimpanCommand => new Command(() =>
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
                        else if (Total == "" || Total == "0")
                        {
                            var toast = Toast.Make("Quantity arrival required!");
                            await toast.Show(cancellationTokenSource.Token);
                        }
                        else if (Photobase64 == "" || Photobase64 == null)
                        {
                            var toast = Toast.Make("Photo product required!");
                            await toast.Show(cancellationTokenSource.Token);
                        }
                        else
                        {
                            if (Int32.Parse(Total) > TotalRemaining)
                            {
                                var toast = Toast.Make("Quantity melebihi remaining");
                                await toast.Show(cancellationTokenSource.Token);
                            }
                            else
                            {
                                var model2 = new DeliveryOrderArrival();
                                model2.DOProductId = ProductData.DOProductId;
                                model2.Quantity = Int32.Parse(Total);
                                model2.Note = Note != null ? Note : "";
                                model2.ProductImage = Photobase64;

                                var response = await deliveryOrdeService.SaveArrival(model2);

                                if (response.Code == System.Net.HttpStatusCode.OK)
                                {
                                    var toast = Toast.Make(response.Data.ToString());
                                    await toast.Show(cancellationTokenSource.Token);

                                    WeakReferenceMessenger.Default.Send(new RefreshCollection("RefreshProductDO"));

                                    await Shell.Current.GoToAsync("..");

                                    //await Shell.Current.GoToAsync($"../..//{nameof(DeliveryOrderDetailPage)}");
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
                        IsBusy = false;
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



        //#if __IOS__
        //        public static byte[] ResizeImageIOS(byte[] imageData, float width, float height)
        //        {
        //            UIImage originalImage = ImageFromByteArray(imageData);
        //            UIImageOrientation orientation = originalImage.Orientation;

        //            //create a 24bit RGB image
        //            using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
        //                                                 (int)width, (int)height, 8,
        //                                                 4 * (int)width, CGColorSpace.CreateDeviceRGB(),
        //                                                 CGImageAlphaInfo.PremultipliedFirst))
        //            {

        //                RectangleF imageRect = new RectangleF(0, 0, width, height);

        //                // draw the image
        //                context.DrawImage(imageRect, originalImage.CGImage);

        //                UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, orientation);

        //                // save the image as a jpeg
        //                return resizedImage.AsJPEG().ToArray();
        //            }
        //        }

        //        public static UIKit.UIImage ImageFromByteArray(byte[] data)
        //        {
        //            if (data == null)
        //            {
        //                return null;
        //            }

        //            UIKit.UIImage image;
        //            try
        //            {
        //                image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine("Image load failed: " + e.Message);
        //                return null;
        //            }
        //            return image;
        //        }
        //#endif

#if __ANDROID__
        public static byte[] ResizeImageAndroid(byte[] imageData, float width, float height)
        {
            // Load the bitmap
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, true);

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                return ms.ToArray();
            }
        }
#endif
    }
}
