using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class ProvinsiRepository : Repository<MasProvinsi>, IProvinsiRepository
    {
        private readonly AppDbContext db;

        public ProvinsiRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(MasProvinsi masProvinsi)
        {
           db.Update(masProvinsi);
        }
    }
}
