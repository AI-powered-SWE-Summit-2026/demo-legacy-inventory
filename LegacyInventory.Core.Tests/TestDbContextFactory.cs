using System;
using LegacyInventory.Core;
using Microsoft.EntityFrameworkCore;

namespace LegacyInventory.Core.Tests
{
    internal static class TestDbContextFactory
    {
        public static InventoryDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase($"inventory-tests-{Guid.NewGuid()}")
                .Options;

            return new InventoryDbContext(options);
        }
    }
}
