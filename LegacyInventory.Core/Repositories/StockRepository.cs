using System.Collections.Generic;
using System.Linq;
using LegacyInventory.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacyInventory.Core.Repositories
{
    public class StockRepository
    {
        private readonly InventoryDbContext _context;

        public StockRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public IList<StockLevel> GetAll()
        {
            return _context.StockLevels
                .Include(x => x.Product)
                .ThenInclude(x => x.Category)
                .Include(x => x.Warehouse)
                .OrderBy(x => x.Product.Name)
                .ThenBy(x => x.Warehouse.Name)
                .ToList();
        }

        public IList<StockLevel> GetLowStock()
        {
            return _context.StockLevels
                .Include(x => x.Product)
                .Include(x => x.Warehouse)
                .Where(x => (x.Quantity - x.ReservedQuantity) <= x.Product.ReorderLevel)
                .OrderBy(x => x.Product.Name)
                .ToList();
        }
    }
}
