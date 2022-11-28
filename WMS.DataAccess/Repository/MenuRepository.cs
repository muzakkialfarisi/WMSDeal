using Microsoft.EntityFrameworkCore;
using WMS.DataAccess;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class MenuRepository : IMenuRepository
    {
        private AppDbContext _db;
        public MenuRepository(AppDbContext db)
        {
            _db = db;
        }
        public IEnumerable<SecMenu> GetAll()
        {
            return _db.SecMenus.Include(x => x.SecProfileMenus).OrderBy(m => m.MenuSort);
        }

        public IEnumerable<SecMenu> Get(int id)
        {
            var profileId = id;

            var menu = _db.SecMenus.FromSqlRaw($"GetMenu {profileId}")
                                  .ToList();

            return menu;
        }
    }
}
