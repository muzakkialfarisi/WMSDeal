using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class SalesOrderCourierRepository : Repository<MasSalesCourier>, ISalesOrderCourierRepository
    {
        private readonly AppDbContext db;

        public SalesOrderCourierRepository(AppDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(MasSalesCourier outSales)
        {
            db.Update(outSales);
        }
    }
}
