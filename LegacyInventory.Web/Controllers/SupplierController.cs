using System;
using System.Linq;
using LegacyInventory.Core;
using LegacyInventory.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LegacyInventory.Web.Controllers
{
    public class SupplierController : Controller
    {
        private readonly InventoryDbContext _context;
        private readonly ILogger<SupplierController> _logger;

        public SupplierController(InventoryDbContext context, ILogger<SupplierController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(_context.Suppliers.OrderBy(x => x.Name).ToList());
        }

        public IActionResult Create()
        {
            return View(new Supplier
            {
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                supplier.CreatedAt = supplier.CreatedAt == default ? DateTime.UtcNow : supplier.CreatedAt;
                _context.Suppliers.Add(supplier);
                _context.SaveChanges();
                Console.WriteLine($"Created supplier {supplier.Name}");
                _logger.LogInformation("Created supplier {SupplierName}", supplier.Name);
                return RedirectToAction(nameof(Index));
            }

            return View(supplier);
        }
    }
}
