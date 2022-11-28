using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class SalesOrderDispatchRepository : Repository<OutSalesDispatchtoCourier>, ISalesOrderDispatchRepository
    {
        private readonly AppDbContext db;

        public SalesOrderDispatchRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(OutSalesDispatchtoCourier outSales)
        {
            db.Update(outSales);
        }
    }
}