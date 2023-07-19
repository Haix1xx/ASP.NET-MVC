using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.Models.Blog;

namespace MVC.Areas.Blog.Controllers
{
    [Area("Blog")]
    public class ViewPostController : Controller
    {
        private readonly ILogger<ViewPostController> _logger;
        private readonly AppDbContext _context;
        public ViewPostController(ILogger<ViewPostController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Route("/post/{categoryslug?}")]
        public async Task<IActionResult> Index(string? categoryslug, int pageSize,[FromQuery(Name = "p")] int currentPage)
        {
            ViewBag.categories = await GetCategory();
            ViewBag.categorySlug = categoryslug;
            Category? category = null;
            if (!string.IsNullOrEmpty(categoryslug))
            {
                category = await _context.Categories.Where(c => c.Slug == categoryslug)
                                                            .Include(c => c.CategoryChildren)
                                                            .FirstOrDefaultAsync();
                if (category == null)
                    return NotFound();
            }

            var posts = _context.Posts.Include(p => p.Author)
                                     .Include(p => p.PostCategories)
                                     .ThenInclude(pc => pc.Category)
                                     .AsQueryable();

            posts.OrderByDescending(p => p.DateUpdated);

            if (category != null)
            {
                var ids = new List<int>();
                category.ChildCategoryIds(ids, category.CategoryChildren);
                ids.Add(category.Id);

                posts = posts.Where(p => p.PostCategories.Where(pc => ids.Contains(pc.CategoryId)).Any());
            }
            //var posts = _context.Posts.Include(p => p.Author);
            int totalPost = await posts.CountAsync();
            if (pageSize <= 0)
                pageSize = 5;
            int countPages = (int)Math.Ceiling((double)totalPost / pageSize);
            if (currentPage > countPages)
                currentPage = countPages;
            if (currentPage <= 0)
                currentPage = 1;

            var pagingmodel = new PagingModel
            {
                currentpage = currentPage,
                countpages = countPages,
                generateUrl = (p) => Url.Action("Index", new { p = p, pagesize = pageSize })
            };
            ViewBag.pagingmodel = pagingmodel;
            ViewBag.Category = category;
            posts = posts.Skip((currentPage - 1) * pageSize)
                         .Take(pageSize);
            return View(await posts.ToListAsync());
        }

        [Route("/post/{postslug}.html")]
        public async Task<IActionResult> Details(string? postslug)
        {
            ViewBag.categories = await GetCategory();

            var post = _context.Posts.Where(p => p.Slug == postslug)
                                     .Include(p => p.Author)
                                     .Include(p => p.PostCategories)
                                     .ThenInclude(pc => pc.Category)
                                     .FirstOrDefault();
            if (post == null)
            {
                return NotFound();
            }
            var category = post.PostCategories.FirstOrDefault()?.Category;
            var otherPosts = _context.Posts.Where(p => p.PostCategories.Any(pc => pc.CategoryId == category.Id))
                                          .Where(p => p.PostId != post.PostId)
                                          .OrderByDescending(p => p.DateCreated)
                                          .Take(5);
            ViewBag.otherPosts = otherPosts;
            ViewBag.category = category;
            return View(post);
        }

        private async Task<IEnumerable<Category>> GetCategory()
        {
            return await _context.Categories
                        .Include(c => c.CategoryChildren)
                        .Where(c => c.ParentCategory == null)
                        .ToListAsync();
        }
    }
}