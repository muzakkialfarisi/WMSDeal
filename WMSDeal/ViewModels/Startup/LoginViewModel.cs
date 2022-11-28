using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Alerts;
using WMSDeal.Constant;
using WMSDeal.Services;
using Newtonsoft.Json;
//using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;
using WMSDeal.Models;
using CommunityToolkit.Mvvm.Input;
using System.IdentityModel.Tokens.Jwt;

namespace WMSDeal.ViewModels.Startup
{
    public partial class LoginViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;

        private readonly ILoginService loginService = new LoginService();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private string _loginText = "SIGN IN";
        public string LoginText
        {
            get => _loginText;
            set => SetProperty(ref _loginText, value);
        }

        bool _isPassword = true;
        public bool isPassword
        {
            get => _isPassword;
            set
            {
                SetProperty(ref _isPassword, value);
                OnPropertyChanged();
            }
        }
        ImageSource _showPassword = Icon.Eyehide;

        public ImageSource ShowPassword
        {
            get => _showPassword;
            set
            {
                _showPassword = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowPasswordCommand { private set; get; }
        public LoginViewModel()
        {
            ShowPasswordCommand = new RelayCommand(Method1);
            _showPassword = Icon.Eyehide;
        }
        #region Commands
        //public ICommand ShowPasswordCommand = new Command(() =>
        //{

        //});
        private void Method1()
        {
            if (ShowPassword.ToString() == "File: eyeshow.png")
            {
                ShowPassword = Icon.Eyehide;
                isPassword = true;
            }
            else
            {
                ShowPassword = Icon.Eyeshow;
                isPassword = false;
            }
        }


        [RelayCommand]
        async void Login()
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    if (Preferences.ContainsKey(nameof(App.UserInfo)))
                    {
                        Preferences.Remove(nameof(App.UserInfo));
                    }

                    var response = await loginService.Login(new LoginRequest
                    {
                        Username = Username,
                        Password = Password,
                    });

                    if (response.Code == System.Net.HttpStatusCode.OK)
                    {
                        try
                        {
                            var json = response.Data.ToString();
                            var model = JsonConvert.DeserializeObject<TokenApi>(json);

                            await SecureStorage.SetAsync(nameof(App.Token), model.Token);

                            var handler = new JwtSecurityTokenHandler();
                            var jsonToken = handler.ReadToken(model.Token) as JwtSecurityToken;

                            UserInfo userInfo = new UserInfo();
                            userInfo.UserId = new Guid(jsonToken.Claims.FirstOrDefault(claim => claim.Type == "UserId").Value);
                            userInfo.UserName = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "UserName").Value;
                            userInfo.FirstName = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "FirstName").Value;
                            userInfo.LastName = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "LastName").Value;
                            userInfo.JobPosName = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "JobPosName").Value;
                            userInfo.Profile = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "ProfileName").Value;
                            userInfo.ProfileId = Int32.Parse(jsonToken.Claims.FirstOrDefault(claim => claim.Type == "ProfileId").Value);
                            userInfo.HouseCode = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "HouseCode").Value;
                            userInfo.Warehouse = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "HouseName").Value;
                            userInfo.ProfileImageUrl = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "ProfileImageUrl").Value;
                            string userInfoJsonStr = JsonConvert.SerializeObject(userInfo);

                            Preferences.Set(nameof(App.UserInfo), userInfoJsonStr);

                            App.UserInfo = userInfo;

                            await AppConstant.AddFlyoutMenusDetails();
                            //await Shell.Current.GoToAsync(nameof(HomePage));
                        }
                        catch (Exception ex)
                        {
                            var toast = Toast.Make("Error: " + ex.Message);
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
                    IsBusy = false;
                }
                else
                {
                    var toast = Toast.Make("Username and Password Required!");
                    await toast.Show(cancellationTokenSource.Token);
                }
            }
            else
            {
                var toast = Toast.Make("Connection Lost...");
                await toast.Show(cancellationTokenSource.Token);
            }
        }
        #endregion
    }
}
