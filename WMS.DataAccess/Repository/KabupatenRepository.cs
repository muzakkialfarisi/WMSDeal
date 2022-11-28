using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class KabupatenRepository : Repository<MasKabupaten>, IKabupatenRepository
    {
        private readonly AppDbContext db;

        public KabupatenRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(MasKabupaten mas)
        {
            db.Update(mas);
        }
    }
}
