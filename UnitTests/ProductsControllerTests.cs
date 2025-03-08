using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers.v1;
using NUnit.Framework;
using Domain.Interfaces;

namespace UnitTests
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private Mock<IProductRepository> _mockProductRepository;
        private ProductService _productService;
        private ProductsController _controller;

        [SetUp]
        public void Setup()
        {
            // Mock the repository
            _mockProductRepository = new Mock<IProductRepository>();

            // Inject the mocked repository into the service
            _productService = new ProductService(_mockProductRepository.Object);

            // Inject the service into the controller
            _controller = new ProductsController(_productService);
        }

        [Test]
        public void GetProducts_ShouldReturnListOfProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductID = 1, ProductName = "Product 1" },
                new Product { ProductID = 2, ProductName = "Product 2" }
            };  

            _mockProductRepository.Setup(repo => repo.GetAllProducts()).Returns(products);

            // Act
            var result = _controller.GetProducts();

            // Assert
            Assert.IsNotNull(result);
            var okResult = result.Result as OkObjectResult; // Fix: Access .Result if ActionResult<T>
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsInstanceOf<List<Product>>(okResult.Value);
        }



    }
}
