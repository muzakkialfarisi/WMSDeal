using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Windows.Input;
using WMSDeal.Constant;
using WMSDeal.Models;
using WMSDeal.Services;
using WMSDeal.Views;
using WMSDeal.Views.Startup;

namespace WMSDeal.ViewModels
{
    public partial class HomeViewModel:BaseViewModel
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        ILoginService loginService = new LoginService();
        public string AppVersi => AppInfo.VersionString;
        ImageSource profileImage = Icon.User;
        #region Total
        int _totalIncoming;
        public int TotalIncoming
        {
            get => _totalIncoming;
            set
            {
                SetProperty(ref _totalIncoming, value);
            }
        }
        int _doneIncoming;
        public int DoneIncoming
        {
            get => _doneIncoming;
            set
            {
                SetProperty(ref _doneIncoming, value);
            }
        }
        int _outstandIncoming;
        public int OutstandIncoming
        {
            get => _outstandIncoming;
            set
            {
                SetProperty(ref _outstandIncoming, value);
            }
        }
        public ImageSource ProfileImage
        {
            get => profileImage;
            set
            {
                SetProperty(ref profileImage, value);
                OnPropertyChanged();
            }
        }

        int _totalOutgoing;
        public int TotalOutgoing
        {
            get => _totalOutgoing;
            set
            {
                SetProperty(ref _totalOutgoing, value);
            }
        }
        int _doneOutgoing;
        public int DoneOutgoing
        {
            get => _doneOutgoing;
            set
            {
                SetProperty(ref _doneOutgoing, value);
            }
        }
        int _outstandOutgoing;
        public int OutstandOutgoing
        {
            get => _outstandOutgoing;
            set
            {
                SetProperty(ref _outstandOutgoing, value);
            }
        }

        #endregion
        public HomeViewModel()
        {
            CheckVersion();
            ProfileImage = AppConstant.BaseUrl + "/img/avatars/" + App.UserInfo.ProfileImageUrl;
            IsBusy = true;
            //App.Current.MainPage.ShowPopup(new LoadingPopupPage());
            Task.Run(async () =>
            {
                await Task.Delay(200);
                HomeDashboard();
            });



        }
        #region Update

        private async void HomeDashboard()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
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
                            var response = await loginService.GetHomeDashboard();
                            if (response.Code == System.Net.HttpStatusCode.OK)
                            {
                                if (response.Data == "")
                                {
                                    var toast = Toast.Make("Data Notfound!");
                                    await toast.Show(cancellationTokenSource.Token);
                                }
                                else
                                {
                                    List<HomeDashboard> homeDashboards = JsonConvert.DeserializeObject<List<HomeDashboard>>(response.Data);
                                    foreach (var item in homeDashboards)
                                    {
                                        if (item.Title == "Incoming")
                                        {
                                            TotalIncoming = item.Total;
                                            DoneIncoming = item.Done;
                                            OutstandIncoming = item.Outstanding;
                                        }
                                        if (item.Title == "Outgoing")
                                        {
                                            TotalOutgoing = item.Total;
                                            DoneOutgoing = item.Done;
                                            OutstandOutgoing = item.Outstanding;
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
                                        //WeakReferenceMessenger.Default.Send(new CloseLoadingPopupMessage("closeloading"));
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
                    // WeakReferenceMessenger.Default.Send(new CloseLoadingPopupMessage("closeloading"));
                });
            }
            else
            {
                var toast = Toast.Make("Connection Lost...");
                await toast.Show(cancellationTokenSource.Token);
            }

            //WeakReferenceMessenger.Default.Send(new CloseLoadingPopupMessage("closeloading"));
        }
        public ICommand RefreshCommand => new Command(() =>
        {
            IsBusy = true;
            // App.Current.MainPage.ShowPopup(new LoadingPopupPage());
            //App.Current.MainPage.ShowPopupAsync(new LoadingPopupPage());
            Task.Run(async () =>
            {
                await Task.Delay(200);
                HomeDashboard();
            });
        });
        public ICommand ProfileCommand => new Command(() =>
        {
            Shell.Current.GoToAsync("UserProfile");
        });
        private async void CheckVersion()
        {
            var response = await loginService.CheckVersion("Android");
            if (response.Code == System.Net.HttpStatusCode.OK)
            {
                if (response.Data != "")
                {
                    AppVersion appVersion = JsonConvert.DeserializeObject<AppVersion>(response.Data);
                    if (appVersion.MinVersion != AppVersi)
                    {
                        App.LinkUpdate = appVersion.Link;
                        await App.Current.MainPage.ShowPopupAsync(new PopupUpdatePage());
                    }
                }
            }
        }
        #endregion

        public ICommand SignOutCommand => new Command(async () =>
        {
            bool action =await Shell.Current.DisplayAlert("Deal", "Are you sure you want to Sign out?", "YES", "NO");
            if (action)
            {
                if (Preferences.ContainsKey(nameof(App.UserInfo)))
                {
                    Preferences.Remove(nameof(App.UserInfo));

                   await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }

            }
        });

    }
}
