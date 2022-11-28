using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class KelurahanRepository : Repository<MasKelurahan>, IKelurahanRepository
    {
        private readonly AppDbContext db;

        public KelurahanRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(MasKelurahan mas)
        {
            db.Update(mas);
        }
    }
}
