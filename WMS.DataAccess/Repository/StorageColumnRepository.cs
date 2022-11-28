using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class StorageColumnRepository : Repository<InvStorageColumn>, IStorageColumnRepository
    {
        private readonly AppDbContext db;

        public StorageColumnRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(InvStorageColumn invStorageColumn)
        {
           db.Update(invStorageColumn);
        }
    }
}
