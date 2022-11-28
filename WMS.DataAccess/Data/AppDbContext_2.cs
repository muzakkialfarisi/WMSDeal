using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Models;

namespace WMS.DataAccess.Data
{
    public class AppDbContext_2 : DbContext
    {
        public AppDbContext_2(DbContextOptions<AppDbContext_2> options) : base(options)
        {
        }
       
    }
}
