using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WMSDeal.Constant;
using WMSDeal.Models;

namespace WMSDeal.Services
{
    public interface ILoginService
    {
        Task<DefaultResponse> Login(LoginRequest loginRequest);
        Task<DefaultResponse> CheckVersion(string Device);
        Task<DefaultResponse> GetHomeDashboard();
        Task<DefaultResponse> GetUser();
    }
    public class LoginService : ILoginService
    {
        public async Task<DefaultResponse> CheckVersion(string Device)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    //var token = await SecureStorage.GetAsync(nameof(App.Token));

                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string response = await client.GetStringAsync(AppConstant.BaseUrl + "/maui/Auth/Versions/Maui/LastVersion");

                    var _response = new DefaultResponse();
                    if (response != null)
                    {
                        _response.Code = HttpStatusCode.OK;
                        _response.Message = "Successful";
                        _response.Data = response;
                        return _response;
                    }
                    else
                    {
                        _response.Code = HttpStatusCode.BadRequest;
                        _response.Message = "Bad Request";
                        _response.Data = "";
                        return _response;
                    }
                }
                catch (Exception ex)
                {
                    var _error = new ErrorResponse();

                    _error.StatusCode = "400";
                    _error.Error = "Error Exception";
                    _error.Message = ex.Message;
                    _error.Code = "LG5001";

                    var _response = new DefaultResponse();
                    _response.Code = HttpStatusCode.BadRequest;
                    _response.Message = "Error Exception";
                    _response.Data = JsonConvert.SerializeObject(_error);
                    return _response;
                }
            }
        }

        public async Task<DefaultResponse> GetHomeDashboard()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string response = await client.GetStringAsync(AppConstant.BaseUrl + "/maui/dashboard/home");

                    var _response = new DefaultResponse();
                    if (response != null)
                    {
                        _response.Code = HttpStatusCode.OK;
                        _response.Message = "Successful";
                        _response.Data = response;
                        return _response;
                    }
                    else
                    {
                        _response.Code = HttpStatusCode.BadRequest;
                        _response.Message = "Bad Request";
                        _response.Data = "";
                        return _response;
                    }
                }
                catch (Exception ex)
                {
                    var _error = new ErrorResponse();

                    _error.StatusCode = "400";
                    _error.Error = "Error Exception";
                    _error.Message = ex.Message;
                    _error.Code = "LG5001";

                    var _response = new DefaultResponse();
                    _response.Code = HttpStatusCode.BadRequest;
                    _response.Message = "Error Exception";
                    _response.Data = JsonConvert.SerializeObject(_error);
                    return _response;
                }
            }

        }

        public async Task<DefaultResponse> GetUser()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var token = await SecureStorage.GetAsync(nameof(App.Token));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string response = await client.GetStringAsync(AppConstant.BaseUrl + "/maui/users/getuser");
                    var _response = new DefaultResponse();
                    if (response != null)
                    {
                        _response.Code = HttpStatusCode.OK;
                        _response.Message = "Successful";
                        _response.Data = response;
                        return _response;
                    }
                    else
                    {
                        _response.Code = HttpStatusCode.BadRequest;
                        _response.Message = "Bad Request";
                        _response.Data = "";
                        return _response;
                    }
                }
                catch (Exception ex)
                {
                    var _error = new ErrorResponse();

                    _error.StatusCode = "400";
                    _error.Error = "Error Exception";
                    _error.Message = ex.Message;
                    _error.Code = "LG5001";

                    var _response = new DefaultResponse();
                    _response.Code = HttpStatusCode.BadRequest;
                    _response.Message = "Error Exception";
                    _response.Data = JsonConvert.SerializeObject(_error);
                    return _response;
                }
            }
        }

        public async Task<DefaultResponse> Login(LoginRequest loginRequest)
        {

            using (var client = new HttpClient())
            {
                try
                {
                    string loginRequestStr = JsonConvert.SerializeObject(loginRequest);

                    var response = await client.PostAsync(AppConstant.BaseUrl + "/maui/auth/login", new StringContent(loginRequestStr, Encoding.UTF8, "application/json"));
                    var _response = new DefaultResponse();
                    if (response.IsSuccessStatusCode)
                    {
                        _response.Code = HttpStatusCode.OK;
                        _response.Message = "Successful";
                        _response.Data = response.Content.ReadAsStringAsync().Result;
                        return _response;
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        _response.Code = response.StatusCode;
                        _response.Message = "Error Exception";
                        _response.Data = json;
                        return _response;
                    }
                    else
                    {
                        _response.Code = response.StatusCode;
                        _response.Message = "Error Exception";
                        _response.Data = "";
                        return _response;
                    }

                    //if (response.StatusCode == HttpStatusCode.OK)
                    //{
                    //    var json = await response.Content.ReadAsStringAsync();
                    //    var model = JsonConvert.DeserializeObject<TokenApi>(json);

                    //    await SecureStorage.SetAsync(nameof(App.Token), model.Token);

                    //    var handler = new JwtSecurityTokenHandler();
                    //    var jsonToken = handler.ReadToken(model.Token) as JwtSecurityToken;

                    //    UserInfo userInfo = new UserInfo();
                    //    userInfo.UserId = new Guid(jsonToken.Claims.FirstOrDefault(claim => claim.Type == "UserId").Value);
                    //    userInfo.UserName = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "UserName").Value;
                    //    userInfo.FirstName = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "FirstName").Value;
                    //    userInfo.LastName = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "LastName").Value;
                    //    userInfo.Email = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "Email").Value;
                    //    userInfo.JobPosName = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "JobPosName").Value;
                    //    userInfo.Profile = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "ProfileName").Value;
                    //    userInfo.ProfileId = Int32.Parse(jsonToken.Claims.FirstOrDefault(claim => claim.Type == "ProfileId").Value);
                    //    userInfo.HouseCode = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "HouseCode").Value;
                    //    userInfo.Warehouse = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "HouseName").Value;

                    //    string userInfoJsonStr = JsonConvert.SerializeObject(userInfo);

                    //    Preferences.Set(nameof(App.UserInfo), userInfoJsonStr);

                    //    App.UserInfo = userInfo;


                    //    _response.Code = HttpStatusCode.OK;
                    //    _response.Message = "Successful";
                    //    _response.Data = json;
                    //    return _response;
                    //}
                    //else if (response.StatusCode == HttpStatusCode.BadRequest)
                    //{
                    //    var json = await response.Content.ReadAsStringAsync();

                    //    _response.Code = response.StatusCode;
                    //    _response.Message = "Error Exception";
                    //    _response.Data = json;
                    //    return _response;
                    //}
                    //else
                    //{
                    //    _response.Code = response.StatusCode;
                    //    _response.Message = "Error Exception";
                    //    _response.Data = "";
                    //    return _response;
                    //}
                }
                catch (Exception msg)
                {
                    var _error = new ErrorResponse();

                    _error.StatusCode = "400";
                    _error.Error = "Error Exception";
                    _error.Message = msg.Message;
                    _error.Code = "LG5001";

                    var _response = new DefaultResponse();
                    _response.Code = HttpStatusCode.BadRequest;
                    _response.Message = "Error Exception";
                    _response.Data = JsonConvert.SerializeObject(_error);
                    return _response;
                }
            }
        }

    }
}
