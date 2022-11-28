using WMS.Models;

namespace WMS.DataAccess.Repository.IRepository
{
    public interface IProductPriorityRepository : IRepository<MasProductPriority>
    {
        void Update(MasProductPriority masProductPriority);
    }
}