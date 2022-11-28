using WMS.Models;

namespace WMS.DataAccess.Repository.IRepository
{
    public interface IStockOpnameRepository : IRepository<InvStockOpname>
    {
        void Update(InvStockOpname inv);
    }
}