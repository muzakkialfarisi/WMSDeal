#nullable disable
using Microsoft.AspNetCore.Mvc;
using WMS.DataAccess;
using WMS.Models;
using Microsoft.AspNetCore.Authorization;
using WMS.DataAccess.Repository.IRepository;
using Newtonsoft.Json;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    [Authorize(Policy = "SuperAdmin")]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public ProfileController(AppDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _unitOfWork.Profile.GetAllAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(int ProfileId, SecProfile model)
        {
            if (model == null)
            {
                TempData["error"] = "Invalid modelstate!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _unitOfWork.Profile.GetSingleOrDefaultAsync(
                disableTracking:
                    false,
                filter:
                    m => m.ProfileId == ProfileId);

            var update = true;

            if (result == null)
            {
                result = new SecProfile();
                result.CreatedBy = User.FindFirst("UserName")?.Value;
                update = false;
            }

            result.ProfileName = model.ProfileName.ToUpper();
            result.Description = model.Description;
            result.Flag = model.Flag;
            result.DateModified = DateTime.Now;
            result.ModifiedBy = User.FindFirst("UserName")?.Value;

            var namechecker = await _unitOfWork.Profile.GetAllAsync(
                filter:
                    m => m.ProfileName == result.ProfileName &&
                    m.ProfileId != ProfileId);

            if (namechecker.Count > 0)
            {
                TempData["error"] = "Name already used!";
                return RedirectToAction(nameof(Index));
            }

            if (update == true)
            {
                _unitOfWork.Profile.Update(result);
                TempData["success"] = "Profile updated successfully!";
            }
            else
            {
                await _unitOfWork.Profile.AddAsync(result);
                TempData["success"] = "Profile created successfully!";
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Manage(int ProfileId)
        {

            var menus = _context.SecMenus 
                .Select(s => new { s.MenuId, s.ParentId, s.MenuName })
                .OrderBy(x => x.MenuId);

            List<TreeViewMenu> nodes =menus.Select(s => new TreeViewMenu
            {
                id = s.MenuId,  
                parent = s.ParentId,
                text = s.MenuName,
                profileid= ProfileId.ToString()
            }).ToList();

            ViewBag.Json = JsonConvert.SerializeObject(nodes);

            SecProfile secProfile = _context.SecProfiles.Find(ProfileId);

            var profile = _context.SecProfileMenus
                         .Select(x => x.MenuId.ToString())
                         .ToArray();

            ViewBag.profile = JsonConvert.SerializeObject(profile);

            return View(secProfile);
        }

        [HttpPost]
        public async Task<ActionResult> ManageAsync(string selectedItems)
        {
            List<TreeViewMenu> items = JsonConvert.DeserializeObject<List<TreeViewMenu>>(selectedItems);

            List<SecProfileMenu> secProfiles = _context.SecProfileMenus.Where(a => a.ProfileId == int.Parse(items[0].profileid)).ToList();
            for (int j = 0; j < secProfiles.Count; j++)
            {
                var del = _context.SecProfileMenus.Find(secProfiles[j].Id);
                _context.SecProfileMenus.Remove(del);
                _context.SaveChanges();
            }

            for (int i = 0; i < items.Count; i++)
            {
                var secProfile = new SecProfileMenu
                {
                    ProfileId = int.Parse(items[i].profileid),
                    MenuId = items[i].id,
                    IsView = '1',
                    IsInsert = '1',
                    IsEdit = '1',
                    IsDelete = '1',
                    IsPrint = '1',
                    Flag = FlagEnum.Active,
                };
                _context.Add(secProfile);
                _context.SaveChanges();

            };
            TempData["success"] = "Profile Updated Successfully";
            return RedirectToAction("Index");
        }
    }
}
