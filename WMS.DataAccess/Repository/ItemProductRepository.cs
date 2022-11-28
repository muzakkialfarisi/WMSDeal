using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class ItemProductRepository : Repository<IncItemProduct>, IItemProductRepository
    {
        private readonly AppDbContext db;

        public ItemProductRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(IncItemProduct incItemProduct)
        {
            db.Update(incItemProduct);
        }
    }
}
