using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DbHelper _dbHelper;

        public ProductRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                var command = new SqlCommand("SELECT ProductID, ProductName FROM Products", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            ProductID = reader.GetInt32(0),
                            ProductName = reader.GetString(1)
                        });
                    }
                }
            }

            return products;
        }

        public Product GetProductById(int id)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                var command = new SqlCommand("SELECT ProductID, ProductName FROM Products WHERE ProductID = @id", connection);
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Product
                        {
                            ProductID = reader.GetInt32(0),
                            ProductName = reader.GetString(1)
                        };
                    }
                }
            }

            return null;
        }

        public void AddProduct(Product product)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                var command = new SqlCommand("INSERT INTO Products (ProductName) VALUES (@name)", connection);
                command.Parameters.AddWithValue("@name", product.ProductName);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteProduct(int id)
        {
            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Products WHERE ProductID = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }

        public List<object> GetProductsWithPackaging()
        {
            var products = new List<object>();

            using (var connection = _dbHelper.GetConnection())
            {
                connection.Open();

                var query = @"
                    SELECT p.ProductID, p.ProductName, 
                           pk.PackagingID, pk.PackagingType, pk.ParentPackagingID
                    FROM Products p
                    LEFT JOIN Packaging pk ON p.ProductID = pk.ProductID
                    ORDER BY p.ProductID, pk.PackagingID";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int productId = reader.GetInt32(0);
                        string productName = reader.GetString(1);

                        // Check if product already exists
                        var product = products.Find(p => (int)((dynamic)p).ProductID == productId);
                        if (product == null)
                        {
                            product = new
                            {
                                ProductID = productId,
                                ProductName = productName,
                                Packaging = new List<object>()
                            };
                            products.Add(product);
                        }

                        // Add packaging if available
                        if (!reader.IsDBNull(2)) // If PackagingID is not null
                        {
                            ((List<object>)((dynamic)product).Packaging).Add(new
                            {
                                PackagingID = reader.GetInt32(2),
                                PackagingType = reader.GetString(3),
                                ParentPackagingID = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4)
                            });
                        }
                    }
                }
            }

            return products;
        }
    }
}
