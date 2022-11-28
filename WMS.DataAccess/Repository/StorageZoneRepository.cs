using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class StorageZoneRepository : Repository<InvStorageZone>,IStorageZoneRepository
    {
        private readonly AppDbContext db;

        public StorageZoneRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(InvStorageZone invStorageZone)
        {
           db.Update(invStorageZone);
        }
    }
}
