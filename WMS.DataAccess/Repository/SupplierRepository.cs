using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class SupplierRepository : Repository<MasSupplierData>, ISupplierRepository
    {
        private readonly AppDbContext db;

        public SupplierRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(MasSupplierData masSupplierData)
        {
            db.Update(masSupplierData);
        }
    }
}
