using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class RoutePickRepository : Repository<InvPickingRoute>, IRoutePickRepository
    {
        private readonly AppDbContext db;

        public RoutePickRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(InvPickingRoute invPickingRoute)
        {
           db.Update(invPickingRoute);
        }
    }
}
