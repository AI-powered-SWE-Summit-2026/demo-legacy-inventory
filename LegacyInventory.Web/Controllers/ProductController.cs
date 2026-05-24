using System;
using LegacyInventory.Core.Models;
using LegacyInventory.Core.Repositories;
using LegacyInventory.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LegacyInventory.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductRepository _productRepository;
        private readonly PricingWcfClient _pricingWcfClient;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            ProductRepository productRepository,
            PricingWcfClient pricingWcfClient,
            ILogger<ProductController> logger)
        {
            _productRepository = productRepository;
            _pricingWcfClient = pricingWcfClient;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(_productRepository.GetAll());
        }

        public IActionResult Details(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.LivePrice = _pricingWcfClient.GetCurrentPrice(id);
            return View(product);
        }

        public IActionResult Create()
        {
            LoadLookups();
            return View(new Product
            {
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepository.Create(product);
                Console.WriteLine($"Created product {product.SKU}");
                _logger.LogInformation("Created product {Sku}", product.SKU);
                return RedirectToAction(nameof(Index));
            }

            LoadLookups();
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            LoadLookups();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepository.Update(product);
                _logger.LogInformation("Updated product {ProductId}", product.Id);
                return RedirectToAction(nameof(Index));
            }

            LoadLookups();
            return View(product);
        }

        private void LoadLookups()
        {
            ViewBag.Categories = _productRepository.GetCategories();
            ViewBag.Suppliers = _productRepository.GetSuppliers();
        }
    }
}
