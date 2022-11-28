using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class PutAwayRepository : Repository<InvProductPutaway>, IPutAwayRepository
    {
        private readonly AppDbContext db;

        public PutAwayRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(InvProductPutaway invProductPutaway)
        {
           db.Update(invProductPutaway);
        }
    }
}
