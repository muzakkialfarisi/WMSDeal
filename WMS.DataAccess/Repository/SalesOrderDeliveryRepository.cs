using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class SalesOrderDeliveryRepository : Repository<OutsalesOrderDelivery>, ISalesOrderDeliveryRepository
    {
        private readonly AppDbContext db;

        public SalesOrderDeliveryRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(OutsalesOrderDelivery outSales)
        {
            db.Update(outSales);
        }
    }
}