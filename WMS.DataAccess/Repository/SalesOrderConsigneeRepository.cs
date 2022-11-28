using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class SalesOrderConsigneeRepository : Repository<OutSalesOrderConsignee>, ISalesOrderConsigneeRepository
    {
        private readonly AppDbContext db;

        public SalesOrderConsigneeRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(OutSalesOrderConsignee outSales)
        {
            db.Update(outSales);
        }
    }
}