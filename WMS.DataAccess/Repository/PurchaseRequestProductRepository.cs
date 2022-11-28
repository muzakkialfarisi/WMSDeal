using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class PurchaseRequestProductRepository : Repository<IncRequestPurchaseProduct>, IPurchaseRequestProductRepository
    {
        private readonly AppDbContext db;

        public PurchaseRequestProductRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(IncRequestPurchaseProduct incRequestPurchaseProduct)
        {
           db.Update(incRequestPurchaseProduct);
        }
    }
}
