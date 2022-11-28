using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    internal class ProductPriorityRepository : Repository<MasProductPriority>, IProductPriorityRepository
    {
        private readonly AppDbContext db;

        public ProductPriorityRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(MasProductPriority masProductPriority)
        {
            db.Update(masProductPriority);
        }
    }
}