using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class SalesOrderAssignRepository : Repository<OutSalesOrderAssign>, ISalesOrderAssignRepository
    {
        private readonly AppDbContext db;

        public SalesOrderAssignRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(OutSalesOrderAssign outSalesOrderAssign)
        {
            db.Update(outSalesOrderAssign);
        }
    }
}