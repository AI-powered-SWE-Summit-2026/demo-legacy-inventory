using System;
using System.Collections.Generic;
using System.Linq;
using LegacyInventory.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacyInventory.Core.Repositories
{
    public class PurchaseOrderRepository
    {
        private readonly InventoryDbContext _context;

        public PurchaseOrderRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public IList<PurchaseOrder> GetAll()
        {
            return _context.PurchaseOrders
                .Include(x => x.Supplier)
                .Include(x => x.Lines)
                .OrderByDescending(x => x.OrderDate)
                .ToList();
        }

        public PurchaseOrder GetById(int id)
        {
            return _context.PurchaseOrders
                .Include(x => x.Supplier)
                .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
                .Include(x => x.Shipments)
                .FirstOrDefault(x => x.Id == id);
        }

        public IList<Supplier> GetSuppliers()
        {
            return _context.Suppliers.OrderBy(x => x.Name).ToList();
        }

        public IList<Product> GetProducts()
        {
            return _context.Products.OrderBy(x => x.Name).ToList();
        }

        public PurchaseOrder Create(PurchaseOrder purchaseOrder, IEnumerable<PurchaseOrderLine> lines)
        {
            purchaseOrder.OrderDate = purchaseOrder.OrderDate == default ? DateTime.UtcNow : purchaseOrder.OrderDate;
            purchaseOrder.Status = string.IsNullOrWhiteSpace(purchaseOrder.Status) ? "Draft" : purchaseOrder.Status;
            purchaseOrder.CreatedByUserId = string.IsNullOrWhiteSpace(purchaseOrder.CreatedByUserId)
                ? "legacy.user"
                : purchaseOrder.CreatedByUserId;

            var orderLines = lines == null ? new List<PurchaseOrderLine>() : lines.ToList();
            foreach (var line in orderLines)
            {
                line.TotalPrice = line.TotalPrice == default ? line.Quantity * line.UnitPrice : line.TotalPrice;
                purchaseOrder.Lines.Add(line);
            }

            purchaseOrder.TotalAmount = purchaseOrder.Lines.Sum(x => x.TotalPrice);
            _context.PurchaseOrders.Add(purchaseOrder);
            _context.SaveChanges();
            return purchaseOrder;
        }

        public IList<Warehouse> GetWarehouses()
        {
            return _context.Warehouses
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToList();
        }

        public bool UpdateStatus(int id, string newStatus, string note = null)
        {
            var purchaseOrder = _context.PurchaseOrders.FirstOrDefault(x => x.Id == id);
            if (purchaseOrder == null || !IsValidTransition(purchaseOrder.Status, newStatus))
            {
                return false;
            }

            purchaseOrder.Status = newStatus;
            if (!string.IsNullOrWhiteSpace(note))
            {
                purchaseOrder.Notes = string.IsNullOrWhiteSpace(purchaseOrder.Notes)
                    ? note
                    : $"{purchaseOrder.Notes}{Environment.NewLine}{note}";
            }

            _context.SaveChanges();
            return true;
        }

        public bool Receive(int id, int warehouseId, DateTime receivedAtUtc)
        {
            var purchaseOrder = _context.PurchaseOrders
                .Include(x => x.Lines)
                .Include(x => x.Shipments)
                .FirstOrDefault(x => x.Id == id);
            if (purchaseOrder == null || !IsValidTransition(purchaseOrder.Status, "Received"))
            {
                return false;
            }

            if (!_context.Warehouses.Any(x => x.Id == warehouseId && x.IsActive))
            {
                return false;
            }

            foreach (var line in purchaseOrder.Lines)
            {
                var stockLevel = _context.StockLevels
                    .FirstOrDefault(x => x.ProductId == line.ProductId && x.WarehouseId == warehouseId);

                if (stockLevel == null)
                {
                    stockLevel = new StockLevel
                    {
                        ProductId = line.ProductId,
                        WarehouseId = warehouseId,
                        Quantity = 0,
                        ReservedQuantity = 0,
                        LastUpdated = receivedAtUtc
                    };
                    _context.StockLevels.Add(stockLevel);
                }

                stockLevel.Quantity += line.Quantity;
                stockLevel.LastUpdated = receivedAtUtc;
            }

            purchaseOrder.Status = "Received";
            purchaseOrder.Notes = string.IsNullOrWhiteSpace(purchaseOrder.Notes)
                ? $"Received on {receivedAtUtc:yyyy-MM-dd} into warehouse {warehouseId}."
                : $"{purchaseOrder.Notes}{Environment.NewLine}Received on {receivedAtUtc:yyyy-MM-dd} into warehouse {warehouseId}.";

            var shipment = purchaseOrder.Shipments.OrderByDescending(x => x.Id).FirstOrDefault();
            if (shipment != null)
            {
                shipment.Status = "Delivered";
                shipment.ShipmentDate = shipment.ShipmentDate ?? receivedAtUtc;
                shipment.EstimatedArrival = receivedAtUtc;
            }

            _context.SaveChanges();
            return true;
        }

        public Shipment UpsertShipment(
            int purchaseOrderId,
            int? shipmentId,
            string trackingNumber,
            string carrierName,
            string status,
            DateTime? estimatedArrivalUtc)
        {
            var purchaseOrderExists = _context.PurchaseOrders.Any(x => x.Id == purchaseOrderId);
            if (!purchaseOrderExists)
            {
                return null;
            }

            Shipment shipment = null;
            if (shipmentId.HasValue && shipmentId.Value > 0)
            {
                shipment = _context.Shipments.FirstOrDefault(x => x.Id == shipmentId.Value && x.PurchaseOrderId == purchaseOrderId);
            }

            if (shipment == null)
            {
                shipment = new Shipment
                {
                    PurchaseOrderId = purchaseOrderId
                };
                _context.Shipments.Add(shipment);
            }

            shipment.TrackingNumber = trackingNumber;
            shipment.CarrierName = carrierName;
            shipment.Status = status;
            shipment.EstimatedArrival = estimatedArrivalUtc;
            shipment.ShipmentDate = shipment.ShipmentDate ?? DateTime.UtcNow;

            _context.SaveChanges();
            return shipment;
        }

        private bool IsValidTransition(string currentStatus, string nextStatus)
        {
            if (string.Equals(currentStatus, nextStatus, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var current = currentStatus?.Trim() ?? string.Empty;
            var next = nextStatus?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(next))
            {
                return false;
            }

            if (string.Equals(current, "Draft", StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(next, "Submitted", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(next, "Cancelled", StringComparison.OrdinalIgnoreCase);
            }

            if (string.Equals(current, "Submitted", StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(next, "Approved", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(next, "Cancelled", StringComparison.OrdinalIgnoreCase);
            }

            if (string.Equals(current, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(next, "Received", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(next, "Cancelled", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
