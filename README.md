# LegacyInventory (App3)

LegacyInventory is an intentionally old-fashioned ASP.NET Core 3.1 MVC inventory and warehouse management sample built to preserve migration issues for a future .NET 8 modernization exercise.

## Projects
- `LegacyInventory.Core` - EF Core 3.1 data model, DbContext, and repository layer.
- `LegacyInventory.Web` - ASP.NET Core 3.1 MVC application using Startup/Program and legacy service registrations.
- `Database` - SQL Server LocalDB creation and seed scripts.

## Legacy patterns to modernize for .NET 8
1. **Deprecated hosting abstraction** - `LegacyInventory.Web\Startup.cs`, `LegacyInventory.Web\Controllers\HomeController.cs` use `IHostingEnvironment`; migrate to `IWebHostEnvironment`.
2. **Deprecated MVC pipeline** - `LegacyInventory.Web\Startup.cs` uses `services.AddMvc()` and `app.UseMvc()`; replace with endpoint routing and `AddControllersWithViews()`.
3. **Verbose EF Core DI registration** - `LegacyInventory.Web\Startup.cs` uses `AddEntityFrameworkSqlServer().AddDbContext(...UseInternalServiceProvider(...))`; simplify to direct `AddDbContext` registration.
4. **Sync-over-async** - `LegacyInventory.Web\Services\ShippingIntegrationService.cs` uses `.Result` on `ExecuteAsync`, `PostAsync`, `ReadAsStringAsync`, and `GetAsync`; `LegacyInventory.Web\Controllers\PurchaseOrderController.cs` uses `.GetAwaiter().GetResult()`.
5. **`new HttpClient()` anti-pattern** - `LegacyInventory.Web\Services\ShippingIntegrationService.cs` should move to `IHttpClientFactory`.
6. **RestSharp v106 API** - `LegacyInventory.Web\Services\ShippingIntegrationService.cs` uses `RestClient`, `RestRequest`, `Method.GET`, and synchronous `Execute` calls that changed in newer RestSharp versions.
7. **WCF client dependency** - `LegacyInventory.Web\Services\PricingWcfClient.cs` uses `ChannelFactory<T>` and sync WCF calls; replace with modern HTTP/gRPC integration.
8. **Console logging mixed with structured logging** - `LegacyInventory.Web\Services\LowStockBackgroundService.cs`, `LegacyInventory.Web\Controllers\ProductController.cs`, `LegacyInventory.Web\Controllers\SupplierController.cs`, and `LegacyInventory.Web\Controllers\PurchaseOrderController.cs` write to both console and `ILogger`.
9. **Direct `ResourceManager` usage** - `LegacyInventory.Web\Services\LocalizationHelper.cs` bypasses `IStringLocalizer`.
10. **Configuration string indexer access** - `LegacyInventory.Web\Startup.cs`, `LegacyInventory.Web\Services\ShippingIntegrationService.cs`, `LegacyInventory.Web\Services\PricingWcfClient.cs`, and `LegacyInventory.Web\Services\LowStockBackgroundService.cs` use `Configuration["..."]` directly instead of strongly-typed options.
11. **`Process.Start()` old overload** - `LegacyInventory.Web\Controllers\HomeController.cs` uses `Process.Start("notepad.exe", reportPath)`.
12. **Tracked EF Core reads** - repository queries in `LegacyInventory.Core\Repositories\*.cs` intentionally omit `AsNoTracking()`.
13. **Newtonsoft.Json everywhere** - `LegacyInventory.Web\Startup.cs`, `LegacyInventory.Web\Services\ShippingIntegrationService.cs`, `LegacyInventory.Web\Views\Home\Index.cshtml`, and `LegacyInventory.Web\Views\Stock\Index.cshtml` rely on Newtonsoft rather than `System.Text.Json`.
14. **No health checks** - `LegacyInventory.Web\Startup.cs` intentionally does not register or map health checks.
15. **Improper background service cancellation** - `LegacyInventory.Web\Services\LowStockBackgroundService.cs` loops forever and delays without the provided `CancellationToken`.

## Database
1. Run `Database\CreateDatabase.sql` against LocalDB.
2. Run `Database\SeedData.sql` to load sample categories, suppliers, warehouses, products, stock levels, purchase orders, lines, and shipments.

## Suggested .NET 8 migration themes
- Replace Startup with the minimal hosting model.
- Move repositories toward async EF Core queries with `AsNoTracking()` for reads.
- Introduce typed options, health checks, and `IHttpClientFactory`.
- Replace WCF and legacy RestSharp dependencies with supported HTTP integrations.
- Migrate localization to `IStringLocalizer` and modern resource usage.
