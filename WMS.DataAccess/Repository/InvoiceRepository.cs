using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class InvoiceRepository:Repository<MasInvoicing>,IInvoiceRepository
    {
        private readonly AppDbContext db;

        public InvoiceRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(MasInvoicing mas)
        {
            db.Update(mas);
        }
    }
}
