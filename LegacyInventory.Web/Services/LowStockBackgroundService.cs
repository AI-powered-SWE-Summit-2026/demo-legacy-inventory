using System;
using System.Threading;
using System.Threading.Tasks;
using LegacyInventory.Core.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LegacyInventory.Web.Services
{
    public class LowStockBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LowStockBackgroundService> _logger;
        private readonly IConfiguration _configuration;

        public LowStockBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<LowStockBackgroundService> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                await CheckLowStockLevels();

                var delayMinutes = 15;
                int.TryParse(_configuration["BackgroundJobs:LowStockIntervalMinutes"], out delayMinutes);
                if (delayMinutes <= 0)
                {
                    delayMinutes = 15;
                }

                await Task.Delay(TimeSpan.FromMinutes(delayMinutes));
            }
        }

        private async Task CheckLowStockLevels()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var stockRepository = scope.ServiceProvider.GetRequiredService<StockRepository>();
                var lowStockItems = stockRepository.GetLowStock();

                foreach (var item in lowStockItems)
                {
                    Console.WriteLine($"Processing stock alert for product {item.ProductId}");
                    _logger.LogInformation("Stock alert sent for {ProductName} in {WarehouseName}", item.Product.Name, item.Warehouse.Name);
                }
            }

            await Task.CompletedTask;
        }
    }
}
