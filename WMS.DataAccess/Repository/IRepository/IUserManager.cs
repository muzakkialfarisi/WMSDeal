using Microsoft.AspNetCore.Http;
using WMS.Models;

namespace WMS.DataAccess.Repository.IRepository
{
    public interface IUserManager
    {
        Task SignIn(HttpContext httpContext, SecUser user, bool isPersistent = false);
        Task SignOut(HttpContext httpContext);
        string JwtGenerator(SecUser secUser, string siginIn_type);
    }
}
