using Microsoft.AspNetCore.Mvc;
using MVC.Services;

namespace MVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ProductServices _productSevices;
        public ProductController(ILogger<ProductController> logger, ProductServices productSevices)
        {
            _logger = logger;
            _productSevices = productSevices;
        }

        [HttpGet("/product-list")]
        public IActionResult Index()
        {
            var products = _productSevices.Products.OrderBy(p => p.Price).ToList();

            return View(products);
        }
    }
}
