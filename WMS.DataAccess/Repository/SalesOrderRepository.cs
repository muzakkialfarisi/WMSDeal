using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class SalesOrderRepository : Repository<OutSalesOrder>, ISalesOrderRepository
    {
        private readonly AppDbContext db;

        public SalesOrderRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(OutSalesOrder outSalesOrder)
        {
            db.Update(outSalesOrder);
        }
    }
}