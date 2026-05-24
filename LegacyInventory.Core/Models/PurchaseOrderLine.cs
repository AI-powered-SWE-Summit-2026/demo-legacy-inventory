namespace LegacyInventory.Core.Models
{
    public class PurchaseOrderLine
    {
        public int Id { get; set; }

        public int PurchaseOrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public virtual PurchaseOrder PurchaseOrder { get; set; }

        public virtual Product Product { get; set; }
    }
}
