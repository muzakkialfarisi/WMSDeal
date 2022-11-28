using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class CustomerDataRepository:Repository<MasCustomerData>,ICustomerDataRepository
    {
        private readonly AppDbContext db;

        public CustomerDataRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(MasCustomerData customerData)
        {
           db.Update(customerData);
        }
    }
}
