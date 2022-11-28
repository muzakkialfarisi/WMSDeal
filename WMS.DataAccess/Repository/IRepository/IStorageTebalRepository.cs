using WMS.Models;

namespace WMS.DataAccess.Repository.IRepository
{
    public interface IStorageTebalRepository : IRepository<InvStorageTebal>
    {
        void Update(InvStorageTebal invStorageTebal);
    }
}