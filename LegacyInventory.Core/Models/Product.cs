using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LegacyInventory.Core.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string SKU { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        public int CategoryId { get; set; }

        public int SupplierId { get; set; }

        [Range(typeof(decimal), "0", "999999")]
        public decimal UnitPrice { get; set; }

        public int ReorderLevel { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual Category Category { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual ICollection<StockLevel> StockLevels { get; set; } = new List<StockLevel>();

        public virtual ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; } = new List<PurchaseOrderLine>();
    }
}
