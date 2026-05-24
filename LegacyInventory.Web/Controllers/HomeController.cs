using System.Diagnostics;
using System.IO;
using System.Linq;
using LegacyInventory.Core;
using LegacyInventory.Web.Models;
using LegacyInventory.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LegacyInventory.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<HomeController> _logger;
        private readonly InventoryDbContext _context;

        public HomeController(IHostingEnvironment env, ILogger<HomeController> logger, InventoryDbContext context)
        {
            _env = env;
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.EnvironmentName = _env.EnvironmentName;
            ViewBag.ProductCount = _context.Products.Count();
            ViewBag.SupplierCount = _context.Suppliers.Count();
            ViewBag.WarehouseCount = _context.Warehouses.Count();
            ViewBag.OpenPurchaseOrders = _context.PurchaseOrders.Count(x => x.Status != "Received" && x.Status != "Cancelled");
            ViewBag.LowStockCount = _context.StockLevels.Count(x => (x.Quantity - x.ReservedQuantity) <= x.Product.ReorderLevel);
            ViewBag.WelcomeMessage = LocalizationHelper.Get("WelcomeMessage");
            ViewBag.SpanishWelcomeMessage = LocalizationHelper.Get("WelcomeMessage", "es-ES");
            ViewBag.DashboardTitle = LocalizationHelper.Get("DashboardTitle");
            _logger.LogInformation("Loaded inventory dashboard in {EnvironmentName}", _env.EnvironmentName);
            return View();
        }

        public IActionResult OpenSeedData()
        {
            var reportPath = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "..", "Database", "SeedData.sql"));
            if (System.IO.File.Exists(reportPath))
            {
                Process.Start("notepad.exe", reportPath);
            }

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
