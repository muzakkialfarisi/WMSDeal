using CommunityToolkit.Maui.Views;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using WMSDeal.Constant;
using WMSDeal.Models;
using WMSDeal.Services;
using WMSDeal.Views;
using WMSDeal.Views.Startup;

namespace WMSDeal.ViewModels.Startup
{
    public partial class LoadingViewModel 
    {
        ILoginService loginService = new LoginService();
        public string AppVersi => AppInfo.VersionString;
        public LoadingViewModel()
        {
            CheckVersion();
        }

        private async void CheckVersion()
        {
            var response = await loginService.CheckVersion("Android");
            if (response.Code == System.Net.HttpStatusCode.OK)
            {
                if (response.Data != "")
                {
                    try
                    {
                        var appVersion = JsonConvert.DeserializeObject<AppVersion>(response.Data);

                        if (appVersion.MinVersion != AppVersi)
                        {
                            App.LinkUpdate = appVersion.Link;
                            await App.Current.MainPage.ShowPopupAsync(new PopupUpdatePage());
                        }
                        else
                        {
                            CheckLoginInfo();
                        }
                    }
                    catch (Exception)
                    {
                        CheckLoginInfo();
                    }
                   
                }
                else
                {
                    CheckLoginInfo();
                }
            }
            else
            {
                CheckLoginInfo();
            }
        }

        private async void CheckLoginInfo()
        {
            string userInfoStr = Preferences.Get(nameof(App.UserInfo), "");

            if (string.IsNullOrWhiteSpace(userInfoStr))
            {
                //Navigate to Login page
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            else
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
                    var userInfo = JsonConvert.DeserializeObject<UserInfo>(userInfoStr);
                    App.UserInfo = userInfo;

                    await AppConstant.AddFlyoutMenusDetails();
                    //await Shell.Current.GoToAsync(nameof(HomePage));
                }
            }
        }
    }
}
