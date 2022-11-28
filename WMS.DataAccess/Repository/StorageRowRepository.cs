using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class StorageRowRepository : Repository<InvStorageRow>, IStorageRowRepository
    {
        private readonly AppDbContext db;

        public StorageRowRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(InvStorageRow invStorageRow)
        {
           db.Update(invStorageRow);
        }
    }
}
