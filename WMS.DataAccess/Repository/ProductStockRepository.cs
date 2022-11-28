using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class ProductStockRepository : Repository<InvProductStock>, IProductStockRepository
    {
        private readonly AppDbContext db;

        public ProductStockRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(InvProductStock invProductStock)
        {
            db.Update(invProductStock);
        }
    }
}
