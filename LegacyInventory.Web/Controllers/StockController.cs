using LegacyInventory.Core.Repositories;
using LegacyInventory.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace LegacyInventory.Web.Controllers
{
    public class StockController : Controller
    {
        private readonly StockRepository _stockRepository;

        public StockController(StockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public IActionResult Index()
        {
            ViewBag.LowStock = _stockRepository.GetLowStock();
            return View(_stockRepository.GetAll());
        }

        public IActionResult Edit(int id)
        {
            var stockLevel = _stockRepository.GetById(id);
            if (stockLevel == null)
            {
                return NotFound();
            }

            return View(stockLevel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(StockLevel stockLevel)
        {
            if (ModelState.IsValid)
            {
                _stockRepository.Update(stockLevel);
                return RedirectToAction(nameof(Index));
            }

            var existing = _stockRepository.GetById(stockLevel.Id);
            if (existing == null)
            {
                return NotFound();
            }

            stockLevel.Product = existing.Product;
            stockLevel.Warehouse = existing.Warehouse;
            return View(stockLevel);
        }
    }
}
