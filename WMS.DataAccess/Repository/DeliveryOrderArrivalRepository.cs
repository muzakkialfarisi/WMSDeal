using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class DeliveryOrderArrivalRepository : Repository<IncDeliveryOrderArrival>, IDeliveryOrderArrivalRepository
    {
        private readonly AppDbContext db;

        public DeliveryOrderArrivalRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(IncDeliveryOrderArrival incDeliveryOrderArrival)
        {
            db.Update(incDeliveryOrderArrival);
        }
    }
}
