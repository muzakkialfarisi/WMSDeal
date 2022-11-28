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
    public class UserWarehouseRepository : Repository<SecUserWarehouse>, IUserWarehouseRepository
    {
        private readonly AppDbContext _db;

        public UserWarehouseRepository(AppDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(SecUserWarehouse secUser)
        {
           _db.Update(secUser);
        }


    }
}
