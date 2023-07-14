using Microsoft.AspNetCore.Mvc;
using MVC.Services;

namespace MVC.Controllers
{
    public class FirstController : Controller
    {
        private readonly ILogger<FirstController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly ProductServices _products;
        public FirstController(ILogger<FirstController> logger, IWebHostEnvironment env, ProductServices products)
        {
            _logger = logger;
            _env = env;
            _products = products;
        }
        [TempData]
        public string StatusMessage { get; set; }
        public IActionResult Index()
        {
            _logger.LogInformation("Log to index of first controller");
            _logger.LogCritical("www");
            return Content("Index");
        }

        public IActionResult Images()
        {
            string ContentRoot = _env.ContentRootPath;
            _logger.LogInformation(ContentRoot);
            string file_path = Path.Combine(ContentRoot, "Files", "anhnenpowerpoint.jpg");
            var images = System.IO.File.ReadAllBytes(file_path);
            return File(images, "image/jpg");
        }

        public IActionResult Privacy()
        {
            var url = Url.Action("Privacy", "Home") ?? "#";
            return LocalRedirect(url);
        }

        public IActionResult HelloView(string? username)
        {

            return View(model: username);
        }

        public IActionResult ViewProduct(int? id)
        {
            var product = _products.Products.Where(p => p.Id == id).FirstOrDefault();
            if (product == null)
            {
                StatusMessage = "Product not found";
                return Redirect(Url.Action("Index", "Home") ?? "#");
            }
            return View(product);

        }
    }
}
