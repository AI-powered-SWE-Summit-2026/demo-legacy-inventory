using System;
using System.Linq;
using LegacyInventory.Core.Models;
using LegacyInventory.Core.Repositories;
using LegacyInventory.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LegacyInventory.Web.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private readonly PurchaseOrderRepository _purchaseOrderRepository;
        private readonly ShippingIntegrationService _shippingIntegrationService;
        private readonly ILogger<PurchaseOrderController> _logger;

        public PurchaseOrderController(
            PurchaseOrderRepository purchaseOrderRepository,
            ShippingIntegrationService shippingIntegrationService,
            ILogger<PurchaseOrderController> logger)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
            _shippingIntegrationService = shippingIntegrationService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(_purchaseOrderRepository.GetAll());
        }

        public IActionResult Create()
        {
            LoadLookups();
            return View(new PurchaseOrder
            {
                OrderDate = DateTime.Today,
                ExpectedDeliveryDate = DateTime.Today.AddDays(7),
                Status = "Draft",
                CreatedByUserId = "legacy.user"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PurchaseOrder purchaseOrder, int productId, int quantity, decimal unitPrice)
        {
            if (ModelState.IsValid)
            {
                var line = new PurchaseOrderLine
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = quantity * unitPrice
                };

                _purchaseOrderRepository.Create(purchaseOrder, new[] { line });
                Console.WriteLine($"Created purchase order {purchaseOrder.OrderNumber}");
                _logger.LogInformation("Created purchase order {OrderNumber}", purchaseOrder.OrderNumber);
                return RedirectToAction(nameof(Index));
            }

            LoadLookups();
            return View(purchaseOrder);
        }

        public IActionResult Details(int id)
        {
            var purchaseOrder = _purchaseOrderRepository.GetById(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            var shipment = purchaseOrder.Shipments.FirstOrDefault();
            if (shipment != null)
            {
                var trackingInfo = _shippingIntegrationService.GetTrackingInfoAsync(shipment.Id).GetAwaiter().GetResult();
                ViewBag.TrackingInfo = trackingInfo;
                Console.WriteLine($"Loaded tracking information for shipment {shipment.Id}");
            }

            return View(purchaseOrder);
        }

        private void LoadLookups()
        {
            ViewBag.Suppliers = _purchaseOrderRepository.GetSuppliers();
            ViewBag.Products = _purchaseOrderRepository.GetProducts();
        }
    }
}
