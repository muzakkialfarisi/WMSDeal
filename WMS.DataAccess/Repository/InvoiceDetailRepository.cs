using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class InvoiceDetailRepository : Repository<MasInvoicingDetail>, IInvoiceDetailRepository
    {
        private readonly AppDbContext db;

        public InvoiceDetailRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }
    }
}
