using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class SerialNumberRepository : Repository<IncSerialNumber>, ISerialNumberRepository
    {
        private readonly AppDbContext db;

        public SerialNumberRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(IncSerialNumber inc)
        {
            db.Update(inc);
        }
    }
}