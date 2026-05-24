using System;

namespace LegacyInventory.Core.Models
{
    public class StockLevel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int WarehouseId { get; set; }

        public int Quantity { get; set; }

        public DateTime LastUpdated { get; set; }

        public int ReservedQuantity { get; set; }

        public virtual Product Product { get; set; }

        public virtual Warehouse Warehouse { get; set; }
    }
}
