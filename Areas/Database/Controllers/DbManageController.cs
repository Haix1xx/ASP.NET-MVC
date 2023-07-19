using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using MVC.Models;
using MVC.Models.Blog;
using MVC.Models.Product;

namespace MVC.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("database/{action=Index}")]
    public class DbManageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public DbManageController(AppDbContext context, UserManager<AppUser> userManager) 
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SeedData()
        {
            await SeedPostCategory();
            await SeedProductCategory();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        private async Task SeedProductCategory()
        {
            _context.Products.RemoveRange(_context.Products.Where(p => p.Content.Contains("[fakeData]")));
            _context.CategoryProducts.RemoveRange(_context.CategoryProducts.Where(c => c.Description.Contains("[fakeData]")));
            
            _context.SaveChanges();
            var fakerCategory = new Faker<CategoryProduct>();
            int cm = 1;
            fakerCategory.RuleFor(c => c.Title, fk => $"Nhóm sản phẩm  {cm++} " + fk.Lorem.Sentence(1, 2).Trim('.'));
            fakerCategory.RuleFor(c => c.Description, fk => fk.Lorem.Sentence(5) + "[fakeData]");
            fakerCategory.RuleFor(c => c.Slug, fk => fk.Lorem.Slug());

            var cate1 = fakerCategory.Generate();
            var cate11 = fakerCategory.Generate();
            var cate12 = fakerCategory.Generate();
            var cate2 = fakerCategory.Generate();
            var cate21 = fakerCategory.Generate();
            var cate211 = fakerCategory.Generate();

            cate11.ParentCategory = cate1;
            cate12.ParentCategory = cate1;

            cate21.ParentCategory = cate2;
            cate211.ParentCategory = cate21;
            var categories = new CategoryProduct[] { cate1, cate11, cate12, cate2, cate21, cate211 };
            await _context.AddRangeAsync(categories);

            //Post
            var rCateIndex = new Random();
            int bv = 1;
            var user = await _userManager.GetUserAsync(this.User);

            var fakerProduct = new Faker<ProductModel>();
            fakerProduct.RuleFor(p => p.AuthorId, f => user.Id);
            fakerProduct.RuleFor(p => p.Content, f => f.Lorem.Paragraphs(5) + "[fakeData]");
            fakerProduct.RuleFor(p => p.Description, f => f.Lorem.Sentence(3));
            fakerProduct.RuleFor(p => p.DateCreated, f => f.Date.Between(new DateTime(2021, 1, 1), new DateTime(2023, 1, 1)));
            fakerProduct.RuleFor(p => p.Published, f => true);
            fakerProduct.RuleFor(p => p.Slug, f => f.Lorem.Slug());
            fakerProduct.RuleFor(p => p.Title, f => $"Sản phẩm {bv++} " + f.Commerce.ProductName());
            fakerProduct.RuleFor(p => p.Price, f => int.Parse(f.Commerce.Price(500, 1000, 0)));

            IList<ProductModel> products = new List<ProductModel>();
            IList<ProductCategoryProduct> product_category = new List<ProductCategoryProduct>();
            for (int i = 0; i < 40; i++)
            {
                var product = fakerProduct.Generate();
                product.DateUpdated = product.DateCreated;
                products.Add(product);
                product_category.Add(new ProductCategoryProduct
                {
                    Product = product,
                    Category = categories[rCateIndex.Next(5)]
                });
            }
            
            await _context.AddRangeAsync(products);
            await _context.AddRangeAsync(product_category);
            await _context.SaveChangesAsync();

        }

        private async Task SeedPostCategory()
        {
            _context.Categories.RemoveRange(_context.Categories.Where(c => c.Description.Contains("[fakeData]")));
            _context.Posts.RemoveRange(_context.Posts.Where(p => p.Content.Contains("[fakeData]")));
            await _context.SaveChangesAsync();

            var fakerCategory = new Faker<Category>();
            int cm = 1;
            fakerCategory.RuleFor(c => c.Title, fk => $"CM{cm++} " + fk.Lorem.Sentence(1, 2).Trim('.'));
            fakerCategory.RuleFor(c => c.Description, fk => fk.Lorem.Sentence(5) + "[fakeData]");
            fakerCategory.RuleFor(c => c.Slug, fk => fk.Lorem.Slug());

            var cate1 = fakerCategory.Generate();
            var cate11 = fakerCategory.Generate();
            var cate12 = fakerCategory.Generate();
            var cate2 = fakerCategory.Generate();
            var cate21 = fakerCategory.Generate();
            var cate211 = fakerCategory.Generate();

            cate11.ParentCategory = cate1;
            cate12.ParentCategory = cate1;

            cate21.ParentCategory = cate2;
            cate211.ParentCategory = cate21;
            var categories = new Category[] {cate1, cate11, cate12, cate2, cate21, cate211};
            await _context.AddRangeAsync(categories);

            //Post
            var rCateIndex = new Random();
            int bv = 1;
            var user = await _userManager.GetUserAsync(this.User);

            var fakerPost = new Faker<Post>();
            fakerPost.RuleFor(p => p.AuthorId, f => user.Id);
            fakerPost.RuleFor(p => p.Content, f => f.Lorem.Paragraphs(5) + "[fakeData]");
            fakerPost.RuleFor(p => p.Description, f => f.Lorem.Sentence(3));
            fakerPost.RuleFor(p => p.DateCreated, f => f.Date.Between(new DateTime(2021, 1, 1), new DateTime(2023,1,1)));
            fakerPost.RuleFor(p => p.Published, f => true);
            fakerPost.RuleFor(p => p.Slug, f => f.Lorem.Slug());
            fakerPost.RuleFor(p => p.Title, f => $"Bài {bv++} " + f.Lorem.Sentence(3, 4).Trim('.'));

            IList<Post> posts = new List<Post>();
            IList<PostCategory> post_category = new List<PostCategory>();
            for (int i = 0; i < 40; i++)
            {
                var post = fakerPost.Generate();
                post.DateUpdated = post.DateCreated;
                posts.Add(post);
                post_category.Add(new PostCategory
                {
                    Post = post,
                    Category = categories[rCateIndex.Next(5)]
                });
            }
            await _context.AddRangeAsync(posts);
            await _context.AddRangeAsync(post_category);
            await _context.SaveChangesAsync();

        }
    }
}