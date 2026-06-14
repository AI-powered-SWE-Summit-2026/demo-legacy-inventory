using System;
using System.Linq;
using LegacyInventory.Core.Models;
using LegacyInventory.Core.Repositories;
using Xunit;

namespace LegacyInventory.Core.Tests
{
    public class PurchaseOrderRepositoryTests
    {
        [Fact]
        public void UpdateStatus_Returns_False_For_Invalid_Transition()
        {
            using var context = TestDbContextFactory.CreateContext();
            context.PurchaseOrders.Add(new PurchaseOrder
            {
                Id = 1,
                OrderNumber = "PO-1",
                SupplierId = 1,
                Status = "Draft"
            });
            context.SaveChanges();

            var repository = new PurchaseOrderRepository(context);

            var updated = repository.UpdateStatus(1, "Received");

            Assert.False(updated);
            Assert.Equal("Draft", context.PurchaseOrders.Single().Status);
        }

        [Fact]
        public void UpsertShipment_Returns_Null_When_Order_Does_Not_Exist()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repository = new PurchaseOrderRepository(context);

            var shipment = repository.UpsertShipment(404, null, "T1", "Carrier", "InTransit", DateTime.UtcNow);

            Assert.Null(shipment);
        }

    }
}
