using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class ProductBundlingDataRepository : Repository<MasProductBundlingData>, IProductBundlingDataRepository
    {
        private readonly AppDbContext db;

        public ProductBundlingDataRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(MasProductBundlingData mas)
        {
            db.Update(mas);
        }
    }
}
