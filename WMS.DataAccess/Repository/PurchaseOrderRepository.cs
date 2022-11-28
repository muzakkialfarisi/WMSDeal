using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class PurchaseOrderRepository : Repository<IncPurchaseOrder>, IPurchaseOrderRepository
    {
        private readonly AppDbContext db;

        public PurchaseOrderRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(IncPurchaseOrder incPurchaseOrder)
        {
           db.Update(incPurchaseOrder);
        }
    }
}
