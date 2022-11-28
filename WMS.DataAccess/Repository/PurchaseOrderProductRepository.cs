using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class PurchaseOrderProductRepository : Repository<IncPurchaseOrderProduct>, IPurchaseOrderProductRepository
    {
        private readonly AppDbContext db;

        public PurchaseOrderProductRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(IncPurchaseOrderProduct incPurchaseOrderProduct)
        {
           db.Update(incPurchaseOrderProduct);
        }
    }
}
