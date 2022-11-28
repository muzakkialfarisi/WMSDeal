using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class RoutePickColumnRepository : Repository<InvPickingRouteColumn>, IRoutePickColumnRepository
    {
        private readonly AppDbContext db;

        public RoutePickColumnRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(InvPickingRouteColumn invPickingRoute)
        {
           db.Update(invPickingRoute);
        }
    }
}
