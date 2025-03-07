using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NLog;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public ActionResult<List<Product>> GetProducts()
        {
            Logger.Info("Fetching all products.");
            return Ok(_productService.GetAllProducts());
        }

        [HttpPost]
        public IActionResult AddProduct([FromBody] Product product)
        {
            _productService.AddProduct(product);
            Logger.Info($"Added new product: {product.ProductName}");
            return CreatedAtAction(nameof(GetProducts), new { id = product.ProductID }, product);
        }
    }
}
