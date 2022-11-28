using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class ProductHistoryRepository : Repository<InvProductHistory>, IProductHistoryRepository
    {
        private readonly AppDbContext db;

        public ProductHistoryRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(InvProductHistory invProductHistory)
        {
            db.Update(invProductHistory);
        }
    }
}
