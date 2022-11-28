using WMS.Models;

namespace WMS.DataAccess.Repository.IRepository
{
    public interface IStorageBesaranRepository : IRepository<InvStorageBesaran>
    {
        void Update(InvStorageBesaran invStorageBesaran);
    }
}