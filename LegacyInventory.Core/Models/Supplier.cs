using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LegacyInventory.Core.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(100)]
        public string ContactName { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
