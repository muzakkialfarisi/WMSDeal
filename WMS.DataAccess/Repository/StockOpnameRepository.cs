using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class StockOpnameRepository : Repository<InvStockOpname>, IStockOpnameRepository
    {
        private readonly AppDbContext db;

        public StockOpnameRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(InvStockOpname inv)
        {
            db.Update(inv);
        }
    }
}