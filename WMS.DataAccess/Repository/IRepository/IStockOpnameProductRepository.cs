using WMS.Models;

namespace WMS.DataAccess.Repository.IRepository
{
    public interface IStockOpnameProductRepository : IRepository<InvStockOpnameProduct>
    {
        void Update(InvStockOpnameProduct invStorageBesaran);
    }
}