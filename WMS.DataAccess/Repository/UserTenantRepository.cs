using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Models.ViewModels;

namespace WMS.DataAccess.Repository
{
    public class UserTenantRepository : Repository<SecUserTenant>, IUserTenantRepository
    {
        private readonly AppDbContext _db;

        public UserTenantRepository(AppDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(SecUserTenant secUser)
        {
           _db.Update(secUser);
        }


    }
}
