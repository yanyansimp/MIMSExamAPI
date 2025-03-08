using Application.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace WebApi.Controllers.v2
{
    [ApiController]
    [ApiVersion(2)]  // ✅ Ensure correct API version
    [Route("api/v{version:apiVersion}/products")]
    public class ProductsV2Controller : ControllerBase
    {
        private readonly ProductService _productService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ProductsV2Controller(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet(Name = "GetProductsV2")]
        public ActionResult<IEnumerable<object>> GetProductsWithPackaging()
        {
            Logger.Info("Fetching products with packaging details (v2).");
            var products = _productService.GetProductsWithPackaging();
            return Ok(products);
        }
    }
}
