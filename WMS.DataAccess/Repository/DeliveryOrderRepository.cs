using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class DeliveryOrderRepository : Repository<IncDeliveryOrder>, IDeliveryOrderRepository
    {
        private readonly AppDbContext db;

        public DeliveryOrderRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(IncDeliveryOrder incDeliveryOrder)
        {
            db.Update(incDeliveryOrder);
        }
    }
}
