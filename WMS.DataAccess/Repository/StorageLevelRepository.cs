using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class StorageLevelRepository : Repository<InvStorageLevel>, IStorageLevelRepository
    {
        private readonly AppDbContext db;

        public StorageLevelRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(InvStorageLevel invStorageLevel)
        {
           db.Update(invStorageLevel);
        }
    }
}
