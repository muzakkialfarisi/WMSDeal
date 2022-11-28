using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Models;
using WMS.Models.ViewModels;

namespace WMS.DataAccess.Repository.IRepository
{
    public interface IUserWarehouseRepository : IRepository<SecUserWarehouse>
    {
        void Update(SecUserWarehouse secUser);

    }
}
