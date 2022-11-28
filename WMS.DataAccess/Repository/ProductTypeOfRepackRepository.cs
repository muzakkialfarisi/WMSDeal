using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class ProductTypeOfRepackRepository : Repository<MasProductTypeOfRepack>, IProductTypeOfRepackRepository
    {
        private readonly AppDbContext db;

        public ProductTypeOfRepackRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(MasProductTypeOfRepack masProductTypeOfRepack)
        {
            db.Update(masProductTypeOfRepack);
        }
    }
}