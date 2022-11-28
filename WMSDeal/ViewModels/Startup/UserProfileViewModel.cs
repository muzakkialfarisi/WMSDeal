using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using WMSDeal.Constant;
using WMSDeal.Models;
using WMSDeal.Services;
using WMSDeal.Views.Startup;

namespace WMSDeal.ViewModels.Startup
{
    public class UserProfileViewModel : BaseViewModel
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ILoginService loginService = new LoginService();
        private UserInfo userInfo;

        public UserInfo UserInfo
        {
            get => userInfo;
            set => SetProperty(ref userInfo, value);
        }


        ImageSource profileImage = Icon.User;
        public ImageSource ProfileImage
        {
            get => profileImage;
            set
            {
                SetProperty(ref profileImage, value);
                OnPropertyChanged();
            }
        }
        ////#region user
        ////string _username;
        ////public string Username
        ////{
        ////    get => _username;
        ////    set
        ////    {
        ////        SetProperty(ref _username, value);
        ////        OnPropertyChanged();
        ////    }
        ////}

        ////string _firstname;
        ////public string Firstname
        ////{
        ////    get => _firstname;
        ////    set
        ////    {
        ////        SetProperty(ref _firstname, value);
        ////        OnPropertyChanged();
        ////    }
        ////}
        ////string _lastname;
        ////public string Lastname
        ////{
        ////    get => _lastname;
        ////    set
        ////    {
        ////        SetProperty(ref _lastname, value);
        ////        OnPropertyChanged();
        ////    }
        ////}
        ////string _phone;
        ////public string Phone
        ////{
        ////    get => _phone;
        ////    set
        ////    {
        ////        SetProperty(ref _phone, value);
        ////        OnPropertyChanged();
        ////    }
        ////}
        ////string _email;
        ////public string Email
        ////{
        ////    get => _email;
        ////    set
        ////    {
        ////        SetProperty(ref _email, value);
        ////        OnPropertyChanged();
        ////    }
        ////}
        ////string _jobpos;
        ////public string JobPos
        ////{
        ////    get => _jobpos;
        ////    set
        ////    {
        ////        SetProperty(ref _jobpos, value);
        ////        OnPropertyChanged();
        ////    }
        ////}
        ////string _profile;
        ////public string Profile
        ////{
        ////    get => _profile;
        ////    set
        ////    {
        ////        SetProperty(ref _profile, value);
        ////        OnPropertyChanged();
        ////    }
        ////}
        ////DateTime _expired;
        ////public DateTime Expired
        ////{
        ////    get => _expired;
        ////    set
        ////    {
        ////        SetProperty(ref _expired, value);
        ////        OnPropertyChanged();
        ////    }
        ////}
        ////string _warehouse;
        ////public string Warehouse
        ////{
        ////    get => _warehouse;
        ////    set
        ////    {
        ////        SetProperty(ref _warehouse, value);
        ////        OnPropertyChanged();
        ////    }
        ////}
        ////#endregion

        public UserProfileViewModel()
        {
            ProfileImage = AppConstant.BaseUrl + "/img/avatars/" + App.UserInfo.ProfileImageUrl;
            IsRefreshing = false;
            IsBusy = true;
            Task.Run(async () =>
            {
                await Task.Delay(500);
                GetUserProfile();
            });
            //GetUser();
        }

        private void GetUserProfile()
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
                            var response = await loginService.GetUser();

                            if (response.Code == System.Net.HttpStatusCode.OK)
                            {
                                if (response.Data == "")
                                {
                                    var toast = Toast.Make("Data Notfound!");
                                    await toast.Show(cancellationTokenSource.Token);
                                }
                                else
                                {
                                    UserInfo model =JsonConvert.DeserializeObject<UserInfo>(response.Data);
                                    UserInfo = model;
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

        private void GetUser()
        {
            var user = App.UserInfo;
            //Username = user.UserName;
            //Firstname = user.FirstName;
            //Lastname = user.LastName;
            //Phone = user.Profile;
            //Email = user.Email;
            //JobPos = user.JobPosName;
            //Profile = user.Profile;
            //Expired = DateTime.Now;
            //Warehouse = user.Warehouse;
        }
    }
}
