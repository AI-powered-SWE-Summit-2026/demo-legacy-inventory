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
    }
}
