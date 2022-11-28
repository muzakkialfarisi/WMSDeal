using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    internal class ProductUnitRepository : Repository<MasUnit>, IProductUnitRepository
    {
        private readonly AppDbContext db;

        public ProductUnitRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(MasUnit masUnit)
        {
            db.Update(masUnit);
        }
    }
}