using System;
using System.ComponentModel.DataAnnotations;

namespace LegacyInventory.Core.Models
{
    public class Shipment
    {
        public int Id { get; set; }

        public int PurchaseOrderId { get; set; }

        public DateTime? ShipmentDate { get; set; }

        [StringLength(100)]
        public string TrackingNumber { get; set; }

        [StringLength(100)]
        public string CarrierName { get; set; }

        [StringLength(25)]
        public string Status { get; set; }

        public DateTime? EstimatedArrival { get; set; }

        public virtual PurchaseOrder PurchaseOrder { get; set; }
    }
}
