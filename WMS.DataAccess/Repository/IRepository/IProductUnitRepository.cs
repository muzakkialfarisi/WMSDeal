using WMS.Models;

namespace WMS.DataAccess.Repository.IRepository
{
    public interface IProductUnitRepository : IRepository<MasUnit>
    {
        void Update(MasUnit masUnit);
    }
}