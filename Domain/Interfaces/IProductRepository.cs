using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetAllProducts();
        List<object> GetProductsWithPackaging();
        Product GetProductById(int id);
        void AddProduct(Product product);
        void DeleteProduct(int id);
    }
}
