using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class MobileAppVersionRepository : Repository<MobileAppVersion>, IMobileAppVersionRepository
    {
        private readonly AppDbContext db;

        public MobileAppVersionRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(MobileAppVersion mobileAppVersion)
        {
            db.Update(mobileAppVersion);
        }
    }
}
