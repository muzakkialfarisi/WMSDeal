using WMS.DataAccess;
using WMS.Models;
using WMS.Models.ViewModels;
using WMS.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WMS.Controllers
{
    [Authorize(Policy = "Cookie")]
    public class PickingRoutesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public PickingRoutesController(AppDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            PickingRouteViewModel route = new PickingRouteViewModel();
            route.invPickingRoutes = await _context.InvPickingRoutes.Include(m => m.MasHouseCode).OrderBy(m => m.Flag).AsNoTracking().ToListAsync();
            return View(route);
        }

        [HttpGet]
        public async Task<IActionResult> IndexWarehouse(string? id)
        {
            var house = await _context.MasHouseCodes.AsNoTracking().ToListAsync();
            return View(house);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string? id, string? routecode)
        {
            ViewData["HouseCode"] = id;

            PickingRouteViewModel route = new PickingRouteViewModel();

            if(routecode != null)
            {
                route.invPickingRoute = await _context.InvPickingRoutes.SingleAsync(m => m.RouteCode == routecode);
            }

            if(routecode != null && !await _context.InvPickingRoutes.AnyAsync(m => m.RouteCode == routecode))
            {
                TempData["url"] = "PickingRoutes";
                return RedirectPermanent("~/NotFound");
            }

            route.invStorageZones = await _context.InvStorageZones.AsNoTracking().ToListAsync();

            route.invPickingRouteColumns = await _context.InvPickingRouteColumns.Include(m => m.InvPickingRoute)
                                                                                    .Include(m => m.InvStorageColumn)
                                                                                        .ThenInclude(m => m.InvStorageRow)
                                                                                            .ThenInclude(m => m.InvStorageZone)
                                                                                                .Where(m => m.RouteCode == routecode)
                                                                                                    .OrderBy(m => m.Order)
                                                                                                        .ToListAsync();

            route.invStorageColumns = await _context.InvStorageColumns.Include(m => m.InvStorageRow)
                                                                        .ThenInclude(m => m.InvStorageZone)
                                                                            .OrderBy(m => m.InvStorageRow.InvStorageZone)
                                                                                .Where(m => m.InvStorageRow.HouseCode == id)
                                                                                    .Where(m => !route.invPickingRouteColumns.Select(c => c.ColumnCode).Contains(m.ColumnCode))
                                                                                        .AsNoTracking().ToListAsync();
            
            return View(route);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PickingRouteViewModel model)
        {
            if(await _context.InvPickingRoutes.AnyAsync(m => m.RouteCode == model.invPickingRoute.HouseCode + "-" + model.invPickingRoute.RouteCode.Trim()))
            {
                TempData["error"] = "Route Code Already Exist!";
                return RedirectToAction("Create", new { id = model.invPickingRoute.HouseCode});
            }

            ViewData["HouseCode"] = model.invPickingRoute.HouseCode;

            await InsertInvPickingRoutes(model, FlagEnum.NonActive);
            
            int Order = await _context.InvPickingRouteColumns.Where(m => m.RouteCode == model.invPickingRoute.RouteCode).CountAsync();

            await InsertInvPickingRouteColumns(model, model.invPickingRoute.HouseCode + "-" + model.invPickingRoute.RouteCode, Order + 1);

            TempData["success"] = "Success!";
            return RedirectToAction("Edit", new {id = model.invPickingRoute.HouseCode, RouteCode = model.invPickingRoute.HouseCode + "-" + model.invPickingRoute.RouteCode });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id, string? routecode)
        {
            if(id == null && routecode == null)
            {
                TempData["error"] = "Something Wrong!";
                return RedirectToAction("Index");
            }

            ViewData["HouseCode"] = id;

            PickingRouteViewModel route = new PickingRouteViewModel();

            if (routecode != null)
            {
                route.invPickingRoute = await _context.InvPickingRoutes.SingleAsync(m => m.RouteCode == routecode);
            }

            if (routecode != null && !await _context.InvPickingRoutes.AnyAsync(m => m.RouteCode == routecode))
            {
                TempData["url"] = "PickingRoutes";
                return RedirectPermanent("~/NotFound");
            }

            route.invStorageZones = await _context.InvStorageZones.AsNoTracking().ToListAsync();

            route.invPickingRouteColumns = await _context.InvPickingRouteColumns.Include(m => m.InvPickingRoute)
                                                                                    .Include(m => m.InvStorageColumn)
                                                                                        .ThenInclude(m => m.InvStorageRow)
                                                                                            .ThenInclude(m => m.InvStorageZone)
                                                                                                .Where(m => m.RouteCode == routecode)
                                                                                                    .OrderBy(m => m.Order)
                                                                                                        .ToListAsync();

            route.invStorageColumns = await _context.InvStorageColumns.Include(m => m.InvStorageRow)
                                                                        .ThenInclude(m => m.InvStorageZone)
                                                                            .OrderBy(m => m.InvStorageRow.InvStorageZone)
                                                                                .Where(m => m.InvStorageRow.HouseCode == id)
                                                                                    .Where(m => !route.invPickingRouteColumns.Select(c => c.ColumnCode).Contains(m.ColumnCode))
                                                                                        .AsNoTracking().ToListAsync();

            return View(route);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PickingRouteViewModel model)
        {

            ViewData["HouseCode"] = model.invPickingRoute.HouseCode;

            if (!await _context.InvPickingRoutes.AnyAsync(m => m.RouteCode == model.invPickingRoute.RouteCode))
            {
                await InsertInvPickingRoutes(model, FlagEnum.NonActive);
            }

            int Order = await _context.InvPickingRouteColumns.Where(m => m.RouteCode == model.invPickingRoute.RouteCode).CountAsync();

            await InsertInvPickingRouteColumns(model, model.invPickingRoute.RouteCode, Order + 1);

            TempData["success"] = "Success!";
            return RedirectToAction("Edit", new { id = model.invPickingRoute.HouseCode, RouteCode = model.invPickingRoute.RouteCode });
        }

        public async Task<IActionResult> InsertInvPickingRoutes(PickingRouteViewModel model, FlagEnum flagEnum)
        {
            InvPickingRoute route = new InvPickingRoute
            {
                RouteCode = model.invPickingRoute.HouseCode + "-" + model.invPickingRoute.RouteCode.Trim(),
                Name = model.invPickingRoute.Name,
                HouseCode = model.invPickingRoute.HouseCode,
                Log = "Create",
                CreatedBy = User.FindFirst("UserName")?.Value,
                DateCreated = DateTime.Now,
                Flag = flagEnum
            };
            await _context.InvPickingRoutes.AddAsync(route);
            await _context.SaveChangesAsync();
            return null;
        }

        public async Task<IActionResult> InsertInvPickingRouteColumns(PickingRouteViewModel model, string RouteCode, int Order)
        {
            InvPickingRouteColumn column = new InvPickingRouteColumn
            {
                RouteCode = RouteCode,
                ColumnCode = model.invPickingRouteColumn.ColumnCode,
                Order = Order
                
            };
            await _context.InvPickingRouteColumns.AddAsync(column);
            await _context.SaveChangesAsync();
            return null;
        }

        public async Task<IActionResult> UpdateInsertInvPickingRouteColumns(PickingRouteViewModel model)
        {
            var now = await _context.InvPickingRouteColumns.Include(m => m.InvPickingRoute).SingleOrDefaultAsync(m => m.RouteColumn == model.invPickingRouteColumn.RouteColumn);

            var rows = await _context.InvPickingRouteColumns.Where(m => m.RouteCode == now.RouteCode).OrderBy(m => m.Order).ToListAsync();

            if(model.invPickingRoute.Log == "up")
            {
                for(int i = 0; i < rows.Count; i++)
                {
                    if (rows[i].RouteColumn == now.RouteColumn)
                    {
                        rows[i - 1].Order = now.Order;
                        rows[i].Order = now.Order - 1;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            else
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    if (rows[i].RouteColumn == now.RouteColumn)
                    {
                        rows[i].Order = now.Order + 1;
                        rows[i + 1].Order = now.Order - 1;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            
            return RedirectToAction("Edit", new { id = now.InvPickingRoute.HouseCode, RouteCode = now.RouteCode });
        }

        [HttpPost]
        public async Task<IActionResult> ActivationUpdate(PickingRouteViewModel model)
        {
            var mainroute = await _context.InvPickingRoutes.Include(m => m.MasHouseCode)
                                                                .Include(m => m.InvPickingRouteColumns)
                                                                    .SingleOrDefaultAsync(m => m.RouteCode == model.invPickingRoute.RouteCode);

            var storagecolumns = await _context.InvStorageColumns.Include(m => m.InvStorageRow)
                                                                        .Where(m => m.InvStorageRow.HouseCode == mainroute.HouseCode)
                                                                            .AsNoTracking().ToListAsync();
            if (mainroute == null)
            {
                TempData["error"] = "Not Found!";
                return RedirectToAction("Index");
            }

            var listroute = await _context.InvPickingRoutes.Where(m => m.HouseCode == mainroute.HouseCode)
                                                                .Where(m => m.RouteCode != mainroute.RouteCode)
                                                                    .ToListAsync();

            if (mainroute.InvPickingRouteColumns.Count != storagecolumns.Count)
            {
                TempData["error"] = "There are still unrouted columns!";
                return RedirectToAction("Index");
            }

            if (model.invPickingRoute.Flag == FlagEnum.Active)
            {
                mainroute.Flag = FlagEnum.NonActive;
                await _context.SaveChangesAsync();
                TempData["success"] = "Success!";
                return RedirectToAction("Index");
            }

            foreach (var item in listroute)
            {
                item.Flag = FlagEnum.NonActive;
                await _context.SaveChangesAsync();
            }
            mainroute.Flag = FlagEnum.Active;
            await _context.SaveChangesAsync();
            TempData["success"] = "Success!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteInvPickingRoutes(string id)
        {
            var temp1 = await _context.InvPickingRouteColumns.Where(m => m.RouteCode == id).ToListAsync();
            foreach(var item in temp1)
            {
                _context.InvPickingRouteColumns.Remove(item);
            }
            await _context.SaveChangesAsync();

            var temp2 = await _context.InvPickingRoutes.SingleOrDefaultAsync(m => m.RouteCode == id);
            _context.InvPickingRoutes.Remove(temp2);
            await _context.SaveChangesAsync();
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteInvPickingRouteColumns(PickingRouteViewModel model)
        {
            var temp = await _context.InvPickingRouteColumns.Include(m => m.InvPickingRoute.MasHouseCode).SingleOrDefaultAsync(m => m.RouteColumn == model.invPickingRouteColumn.RouteColumn);
            _context.InvPickingRouteColumns.Remove(temp);
            await _context.SaveChangesAsync();

            if (! await _context.InvPickingRouteColumns.Include(m => m.InvPickingRoute).AnyAsync(m => m.RouteCode == temp.RouteCode))
            {
                await DeleteInvPickingRoutes(temp.RouteCode);
                return RedirectToAction("Index");
            }

            var order = await _context.InvPickingRouteColumns.Where(m => m.Order > temp.Order).ToListAsync();
            foreach (var item in order)
            {
                item.Order = item.Order - 1;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Edit", new { id = temp.InvPickingRoute.HouseCode, RouteCode = temp.RouteCode });
        }
    }
}