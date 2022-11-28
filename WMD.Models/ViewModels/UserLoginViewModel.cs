using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models.ViewModels
{
    public class UserLoginViewModel
    {
        public string username { get; set; }

        public string password { get; set; }
    }


    public class UserInfoViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string Email { get; set; }= string.Empty;
        public string JobPosName { get; set; } = string.Empty;
        public string Profile { get; set; } = string.Empty;
        public string HouseCode { get; set; } = string.Empty;
        public string Warehouse { get; set; } = string.Empty;
    }
    public class LoginResponseViewModel
    {
        public string Token { get; set; }
        public UserInfoViewModel UserInfo { get; set; }=new UserInfoViewModel();
    }
    public class UserChangePasswordViewModel
    {
        public Guid UserId { get; set; }
        public string Salt { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty; 
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
