using LegacyInventory.Core.Repositories;
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
    }
}
