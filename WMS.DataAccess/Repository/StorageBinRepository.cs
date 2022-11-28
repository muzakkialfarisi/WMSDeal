using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class StorageBinRepository : Repository<InvStorageBin>, IStorageBinRepository
    {
        private readonly AppDbContext db;

        public StorageBinRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(InvStorageBin invStorageBin)
        {
            db.Update(invStorageBin);
        }
    }
}