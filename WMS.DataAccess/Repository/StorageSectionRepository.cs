using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class StorageSectionRepository : Repository<InvStorageSection>, IStorageSectionRepository
    {
        private readonly AppDbContext db;

        public StorageSectionRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(InvStorageSection invStorageSection)
        {
           db.Update(invStorageSection);
        }
    }
}
