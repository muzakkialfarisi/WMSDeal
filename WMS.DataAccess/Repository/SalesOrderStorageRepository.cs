using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class SalesOrderStorageRepository : Repository<OutSalesOrderStorage>, ISalesOrderStorageRepository
    {
        private readonly AppDbContext db;

        public SalesOrderStorageRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(OutSalesOrderStorage outSalesOrderStorage)
        {
            db.Update(outSalesOrderStorage);
        }
    }
}