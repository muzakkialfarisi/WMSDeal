using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class StorageTebalRepository : Repository<InvStorageTebal>, IStorageTebalRepository
    {
        private readonly AppDbContext db;

        public StorageTebalRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(InvStorageTebal invStorageTebal)
        {
            db.Update(invStorageTebal);
        }
    }
}