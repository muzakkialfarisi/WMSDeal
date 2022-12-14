using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class StorageSizeRepository : Repository<InvStorageSize>,IStorageSizeRepository
    {
        private readonly AppDbContext db;

        public StorageSizeRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(InvStorageSize invStorageSize)
        {
           db.Update(invStorageSize);
        }
    }
}
