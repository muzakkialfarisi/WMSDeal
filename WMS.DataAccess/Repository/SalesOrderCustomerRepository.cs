using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class SalesOrderCustomerRepository : Repository<OutSalesOrderCustomer>, ISalesOrderCustomerRepository
    {
        private readonly AppDbContext db;

        public SalesOrderCustomerRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(OutSalesOrderCustomer outSales)
        {
            db.Update(outSales);
        }
    }
}