using WMS.Models;

namespace WMS.DataAccess.Repository.IRepository
{
    public interface IProductTypeOfRepackRepository : IRepository<MasProductTypeOfRepack>
    {
        void Update(MasProductTypeOfRepack masProductTypeOfRepack);
    }
}