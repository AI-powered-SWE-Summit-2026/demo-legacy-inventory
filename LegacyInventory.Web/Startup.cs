using LegacyInventory.Core;
using LegacyInventory.Core.Repositories;
using LegacyInventory.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LegacyInventory.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlServer()
                    .AddDbContext<InventoryDbContext>((serviceProvider, options) =>
                    {
                        options.UseSqlServer(Configuration["ConnectionStrings:InventoryDb"])
                               .UseInternalServiceProvider(serviceProvider);
                    });

            services.AddMvc(options => { options.EnableEndpointRouting = false; })
                    .AddNewtonsoftJson();

            services.AddHostedService<LowStockBackgroundService>();
            services.AddScoped<ProductRepository>();
            services.AddScoped<StockRepository>();
            services.AddScoped<PurchaseOrderRepository>();
            services.AddSingleton<ShippingIntegrationService>();
            services.AddSingleton<PricingWcfClient>();
            services.AddSingleton<LocalizationHelper>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
