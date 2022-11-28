using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class PurchaseRequestRepository : Repository<IncRequestPurchase>, IPurchaseRequestRepository
    {
        private readonly AppDbContext db;

        public PurchaseRequestRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(IncRequestPurchase incRequestPurchase)
        {
           db.Update(incRequestPurchase);
        }
    }
}
