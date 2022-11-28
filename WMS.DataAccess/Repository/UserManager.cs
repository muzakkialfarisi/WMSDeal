using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Utility;

namespace WMS.DataAccess
{
    public class UserManager : IUserManager
    {
        private readonly DateTime _expires = DateTime.UtcNow.AddHours(12);
        private readonly Jwt _jwt = new Jwt();
        private readonly IConfiguration _configuration;

        public UserManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SignIn(HttpContext httpContext, SecUser user, bool isPersistent = false)
        {
            string authenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            var claims = GetUserClaims(user, SignIn_Type.Internal);

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, authenticationScheme);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = _expires,
            };

            await httpContext.SignInAsync(authenticationScheme, claimsPrincipal, authProperties);
        }

        public async Task SignOut(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public string JwtGenerator(SecUser user, string siginIn_type)
        {
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.key));

            var token = new JwtSecurityToken(
                _jwt.Issuer,
                _jwt.Audience,
                claims: GetUserClaims(user, siginIn_type),
                expires: _expires,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha512)
            );
            return jwtSecurityTokenHandler.WriteToken(token);
        }

        private List<Claim> GetUserClaims(SecUser user, string siginIn_type)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, _jwt.Subject));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()));

            claims.Add(new Claim("UserId", user.UserId.ToString()));
            claims.Add(new Claim("ProfileId", user.ProfileId.ToString()));
            claims.Add(new Claim("UserName", user.UserName));

            if (siginIn_type == SignIn_Type.Internal)
            {
                claims.Add(new Claim("FirstName", user.FirstName));
                claims.Add(new Claim("LastName", user.LastName));
                claims.Add(new Claim("ProfileImageUrl", user.ProfileImageUrl));
                claims.Add(new Claim("HouseCode", user.HouseCode));
                claims.Add(new Claim("HouseName", user.MasHouseCode.HouseName));
                claims.Add(new Claim("ProfileName", user.SecProfile.ProfileName));
                claims.Add(new Claim("JobPosName", user.MasJabatan.JobPosName));
            }
            else
            {
                claims.Add(new Claim("TenantId", user.SecUserTenant.TenantId.ToString()));
                claims.Add(new Claim("TenantName", user.SecUserTenant.MasDataTenant.Name.ToString()));
            }

            return claims;
        }
    }
}
