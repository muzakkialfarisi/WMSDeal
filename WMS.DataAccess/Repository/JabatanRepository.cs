using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class JabatanRepository : Repository<MasJabatan>, IJabatanRepository
    {
        private readonly AppDbContext db;

        public JabatanRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(MasJabatan masJabatan)
        {
            db.Update(masJabatan);
        }
    }
}
