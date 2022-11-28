using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class StorageBesaranRepository : Repository<InvStorageBesaran>, IStorageBesaranRepository
    {
        private readonly AppDbContext db;

        public StorageBesaranRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(InvStorageBesaran invStorageBesaran)
        {
            db.Update(invStorageBesaran);
        }
    }
}