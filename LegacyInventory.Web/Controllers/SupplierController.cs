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

        public IActionResult Edit(int id)
        {
            var supplier = _context.Suppliers.FirstOrDefault(x => x.Id == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.Suppliers.FirstOrDefault(x => x.Id == supplier.Id);
                if (existing == null)
                {
                    return NotFound();
                }

                existing.Name = supplier.Name;
                existing.ContactName = supplier.ContactName;
                existing.Email = supplier.Email;
                existing.Phone = supplier.Phone;
                existing.Address = supplier.Address;
                existing.Country = supplier.Country;
                existing.IsActive = supplier.IsActive;
                _context.SaveChanges();
                _logger.LogInformation("Updated supplier {SupplierId}", supplier.Id);
                return RedirectToAction(nameof(Index));
            }

            return View(supplier);
        }
    }
}
