using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class KecamatanRepository : Repository<MasKecamatan>, IKecamatanRepository
    {
        private readonly AppDbContext db;

        public KecamatanRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(MasKecamatan mas)
        {
            db.Update(mas);
        }
    }
}
