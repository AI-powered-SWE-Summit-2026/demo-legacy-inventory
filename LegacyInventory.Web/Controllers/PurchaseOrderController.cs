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

            ViewBag.Warehouses = _purchaseOrderRepository.GetWarehouses();

            var shipment = purchaseOrder.Shipments.FirstOrDefault();
            if (shipment != null)
            {
                var trackingInfo = _shippingIntegrationService.GetTrackingInfoAsync(shipment.Id).GetAwaiter().GetResult();
                ViewBag.TrackingInfo = trackingInfo;
                Console.WriteLine($"Loaded tracking information for shipment {shipment.Id}");
            }

            return View(purchaseOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(int id)
        {
            if (!_purchaseOrderRepository.UpdateStatus(id, "Submitted", "Submitted for approval."))
            {
                return NotFound();
            }

            _logger.LogInformation("Submitted purchase order {PurchaseOrderId}", id);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            if (!_purchaseOrderRepository.UpdateStatus(id, "Approved", "Approved for receiving."))
            {
                return NotFound();
            }

            _logger.LogInformation("Approved purchase order {PurchaseOrderId}", id);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            if (!_purchaseOrderRepository.UpdateStatus(id, "Cancelled", "Order cancelled."))
            {
                return NotFound();
            }

            _logger.LogInformation("Cancelled purchase order {PurchaseOrderId}", id);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Receive(int id, int warehouseId)
        {
            if (!_purchaseOrderRepository.Receive(id, warehouseId, DateTime.UtcNow))
            {
                return NotFound();
            }

            _logger.LogInformation("Received purchase order {PurchaseOrderId} into warehouse {WarehouseId}", id, warehouseId);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpsertShipment(int id, int? shipmentId, string trackingNumber, string carrierName, string status, DateTime? estimatedArrival)
        {
            var shipment = _purchaseOrderRepository.UpsertShipment(id, shipmentId, trackingNumber, carrierName, status, estimatedArrival);
            if (shipment == null)
            {
                return NotFound();
            }

            _logger.LogInformation("Updated shipment {ShipmentId} for purchase order {PurchaseOrderId}", shipment.Id, id);
            return RedirectToAction(nameof(Details), new { id });
        }

        private void LoadLookups()
        {
            ViewBag.Suppliers = _purchaseOrderRepository.GetSuppliers();
            ViewBag.Products = _purchaseOrderRepository.GetProducts();
        }
    }
}
