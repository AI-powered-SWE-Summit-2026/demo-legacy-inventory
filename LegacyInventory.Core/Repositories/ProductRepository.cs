using System;
using System.Collections.Generic;
using System.Linq;
using LegacyInventory.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacyInventory.Core.Repositories
{
    public class ProductRepository
    {
        private readonly InventoryDbContext _context;

        public ProductRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public IList<Product> GetAll()
        {
            return _context.Products
                .Include(x => x.Category)
                .Include(x => x.Supplier)
                .OrderBy(x => x.Name)
                .ToList();
        }

        public Product GetById(int id)
        {
            return _context.Products
                .Include(x => x.Category)
                .Include(x => x.Supplier)
                .Include(x => x.StockLevels)
                .ThenInclude(x => x.Warehouse)
                .FirstOrDefault(x => x.Id == id);
        }

        public IList<Category> GetCategories()
        {
            return _context.Categories
                .OrderBy(x => x.Name)
                .ToList();
        }

        public IList<Supplier> GetSuppliers()
        {
            return _context.Suppliers
                .OrderBy(x => x.Name)
                .ToList();
        }

        public Product Create(Product product)
        {
            product.CreatedAt = product.CreatedAt == default ? DateTime.UtcNow : product.CreatedAt;
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Add(product);
            _context.SaveChanges();
            return product;
        }

        public void Update(Product product)
        {
            var existing = _context.Products.First(x => x.Id == product.Id);
            existing.Name = product.Name;
            existing.SKU = product.SKU;
            existing.Description = product.Description;
            existing.CategoryId = product.CategoryId;
            existing.SupplierId = product.SupplierId;
            existing.UnitPrice = product.UnitPrice;
            existing.ReorderLevel = product.ReorderLevel;
            existing.IsActive = product.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
}
