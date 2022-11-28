using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class SalesOrderPackRepository : Repository<OutSalesOrderPack>, ISalesOrderPackRepository
    {
        private readonly AppDbContext db;

        public SalesOrderPackRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(OutSalesOrderPack outSales)
        {
            db.Update(outSales);
        }
    }
}