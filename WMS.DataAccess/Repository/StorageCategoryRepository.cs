using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class StorageCategoryRepository : Repository<InvStorageCategory>, IStorageCategoryRepository
    {
        private readonly AppDbContext db;

        public StorageCategoryRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(InvStorageCategory invStorageCategory)
        {
           db.Update(invStorageCategory);
        }
    }
}
