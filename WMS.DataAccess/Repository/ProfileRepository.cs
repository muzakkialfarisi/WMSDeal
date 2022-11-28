using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class ProfileRepository : Repository<SecProfile>, IProfileRepository
    {
        private readonly AppDbContext db;

        public ProfileRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(SecProfile secProfile)
        {
           db.Update(secProfile);
        }
    }
}
