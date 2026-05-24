using System.Linq;
using LegacyInventory.Core;
using LegacyInventory.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LegacyInventory.Web.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly InventoryDbContext _context;
        private readonly ILogger<WarehouseController> _logger;

        public WarehouseController(InventoryDbContext context, ILogger<WarehouseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(_context.Warehouses.OrderBy(x => x.Name).ToList());
        }

        public IActionResult Create()
        {
            return View(new Warehouse
            {
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Warehouse warehouse)
        {
            if (ModelState.IsValid)
            {
                _context.Warehouses.Add(warehouse);
                _context.SaveChanges();
                _logger.LogInformation("Created warehouse {WarehouseName}", warehouse.Name);
                return RedirectToAction(nameof(Index));
            }

            return View(warehouse);
        }
    }
}
