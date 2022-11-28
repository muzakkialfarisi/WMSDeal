using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class DeliveryOrderArrivalProductRepository : Repository<IncDeliveryOrderArrivalProduct>, IDeliveryOrderArrivalProductRepository
    {
        private readonly AppDbContext db;

        public DeliveryOrderArrivalProductRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(IncDeliveryOrderArrivalProduct inc)
        {
            db.Update(inc);
        }
    }
}
