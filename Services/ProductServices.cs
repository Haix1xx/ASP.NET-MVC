using MVC.Models;

namespace MVC.Services
{
    public class ProductServices
    {
        public IList<Product> Products { get; set; }
        public ProductServices() 
        {
            Products = new List<Product>();
            Products.Add(new Product { Id = 1, Name = "IPhone", Price = 244.9 });
            Products.Add(new Product { Id = 2, Name = "Samsung", Price = 214.9 });
            Products.Add(new Product { Id = 3, Name = "Xiaomi", Price = 191.9 });
            Products.Add(new Product { Id = 4, Name = "Oppo", Price = 99.9 });
        }
        public IList<Product> GetProducts() => Products;
    }
}
