using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Models;

namespace WMS.DataAccess.Repository.IRepository
{
    public interface IStorageColumnRepository : IRepository<InvStorageColumn>
    {
        void Update(InvStorageColumn invStorageColumn);
    }
}
