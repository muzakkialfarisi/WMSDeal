using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class StoreRepository : Repository<MasStore>, IStoreRepository
    {
        private readonly AppDbContext db;

        public StoreRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(MasStore mas)
        {
           db.Update(mas);
        }
    }
}
