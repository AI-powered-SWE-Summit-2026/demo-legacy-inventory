using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LegacyInventory.Core.Models
{
    public class PurchaseOrder
    {
        public int Id { get; set; }

        public int SupplierId { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; }

        [Required]
        [StringLength(25)]
        public string Status { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        public decimal TotalAmount { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }

        [StringLength(100)]
        public string CreatedByUserId { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual ICollection<PurchaseOrderLine> Lines { get; set; } = new List<PurchaseOrderLine>();

        public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}
