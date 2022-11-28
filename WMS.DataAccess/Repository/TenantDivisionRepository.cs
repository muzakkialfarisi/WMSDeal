using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class TenantDivisionRepository : Repository<MasDataTenantDivision>, ITenantDivisionRepository
    {
        private readonly AppDbContext db;

        public TenantDivisionRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(MasDataTenantDivision masDataTenantDivision)
        {
           db.Update(masDataTenantDivision);
        }
    }
}
