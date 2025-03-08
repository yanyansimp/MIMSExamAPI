using Application.Services;
using Asp.Versioning;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NLog;

namespace WebApi.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion(1)]  // Ensure correct API version
    [Route("api/v{version:apiVersion}/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet(Name = "GetProductsV1")]
        public ActionResult<List<Product>> GetProducts()
        {
            Logger.Info("Fetching products (v1).");
            return Ok(_productService.GetAllProducts());
        }

        [HttpPost(Name = "AddProductV1")]
        public IActionResult AddProduct([FromBody] Product product)
        {
            _productService.AddProduct(product);
            Logger.Info($"Added new product: {product.ProductName}");
            return CreatedAtAction(nameof(GetProducts), new { id = product.ProductID }, product);
        }
    }
}
