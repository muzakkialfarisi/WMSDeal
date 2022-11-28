using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class StockOpnameProductRepository : Repository<InvStockOpnameProduct>, IStockOpnameProductRepository
    {
        private readonly AppDbContext db;

        public StockOpnameProductRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(InvStockOpnameProduct inv)
        {
            db.Update(inv);
        }
    }
}