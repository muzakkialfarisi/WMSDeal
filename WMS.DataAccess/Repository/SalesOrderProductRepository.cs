using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class SalesOrderProductRepository : Repository<OutSalesOrderProduct>, ISalesOrderProductRepository
    {
        private readonly AppDbContext db;

        public SalesOrderProductRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(OutSalesOrderProduct outSalesOrderProduct)
        {
            db.Update(outSalesOrderProduct);
        }
    }
}