using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC.Areas.Product.Models;
using MVC.Data;
using MVC.Models;
using MVC.Models.Product;

namespace MVC.Areas.Product.Controllers
{
    [Area("Product")]
    [Route("admin/productmange/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator + "," + RoleName.Editor)]
    public class ProductManageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProductManageController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [TempData]
        public string StatusMessage { get; set; } = default!;
        // GET: Blog/Post
        public async Task<IActionResult> Index([FromQuery(Name = "p")]int currentPage, int pageSize)
        {
            //var posts = _context.Posts.Include(p => p.Author);
            int totalPost = await _context.Products.CountAsync();
            if (pageSize <= 0)
                pageSize = 10;
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
            ViewBag.postIndex = (currentPage- 1) * pageSize;
            var products = _context.Products
                                        .OrderByDescending(p => p.DateCreated)
                                        .Skip((currentPage - 1) * pageSize)
                                        .Take(pageSize)
                                        .Include(p => p.ProductCategoryProducts)
                                        .ThenInclude(pc => pc.Category); 
            return View(await products.ToListAsync());
        }

        // GET: Blog/Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Blog/Post/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _context.CategoryProducts.ToListAsync();
            ViewData["Categories"] = new MultiSelectList(categories, "Id", "Title");
            return View();
        }

        // POST: Blog/Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,Published,CategoryIDs,Price")] CreateProductModel product)
        {
            if (await _context.Products.AnyAsync(p => p.Slug == product.Slug))
            {
                ModelState.AddModelError("Slug", "Slug này đã được sử dụng");
                return View(product);
            }    
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                product.DateCreated = DateTime.Now;
                product.AuthorId = user.Id;
                _context.Add(product);
                if (product.CategoryIDs != null)
                {
                    foreach(var categoryId in product.CategoryIDs)
                    {
                        await _context.AddAsync(new ProductCategoryProduct
                        {
                            CategoryId = categoryId,
                            Product = product
                        });
                    }    
                }
                StatusMessage = "Vừa tạo bài viết mới";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var categories = await _context.CategoryProducts.ToListAsync();
            ViewData["Categories"] = new MultiSelectList(categories, "Id", "Title");
            return View(product);
        }

        // GET: Blog/Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var post = await _context.Products.AsNoTracking().Include(p => p.ProductCategoryProducts).AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
            if (post == null)
            {
                return NotFound();
            }
            var postEdit = new CreateProductModel(post);
            var categories = await _context.CategoryProducts.ToListAsync();
            ViewData["Categories"] = new MultiSelectList(categories, "Id", "Title");
            return View(postEdit);
        }

        // POST: Blog/Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Title,Description,Slug,Content,Published,CategoryIDs,Price")] CreateProductModel product)
        {
            if (id != product.ProductId)
            {
                return NotFound("1");
            }

            if (await _context.Products.AnyAsync(p => p.Slug == product.Slug && p.ProductId != id))
            {
                ModelState.AddModelError(string.Empty, "Nhập slug khác");
                return View(product);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var productUpdate = await _context.Products.Include(p => p.ProductCategoryProducts).FirstOrDefaultAsync(p => p.ProductId == id);
                    if (productUpdate == null)
                        return NotFound("2");

                    productUpdate.Title = product.Title;
                    productUpdate.Description = product.Description;
                    productUpdate.Content = product.Content;
                    productUpdate.Published = product.Published;
                    productUpdate.Slug = product.Slug;
                    productUpdate.DateUpdated = DateTime.Now;
                    productUpdate.Price = product.Price;
                    // cap nhat post categories
                    if (product.CategoryIDs == null)
                        product.CategoryIDs = new int[] { } ;

                    var oldCateIds = productUpdate.ProductCategoryProducts.Select(pc => pc.ProductId).ToArray();
                    var newCateIds = product.CategoryIDs;
                    //var intersect = oldCateIds.Intersect(newCateIds);
                    //oldCateIds = oldCateIds.Except(intersect).ToArray();
                    //newCateIds = newCateIds.Except(intersect).ToArray();
                    var removeCatePosts = productUpdate.ProductCategoryProducts.Where(p => !newCateIds.Contains(p.CategoryId));
                    //var removeCatePosts = from postCate in postUpdate.PostCategories
                    //                    where (!newCateIds.Contains(postCate.CategoryId))
                    //                    select postCate;
                    _context.RemoveRange(removeCatePosts);
                    _context.SaveChanges();
                    //var addIds = from CateId in newCateIds
                    //             where (!oldCateIds.Contains(CateId))
                    //             select CateId;
                    var addIds = newCateIds.Where(p => !oldCateIds.Contains(p));

                    
                    foreach(var CateId in addIds)
                    {
                        _context.Add(new ProductCategoryProduct
                        {
                            CategoryId = CateId,
                            ProductId = id
                        });
                    }
                    _context.Update(productUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var categories = await _context.ProductCategoryProducts.AsNoTracking().ToListAsync();
            ViewData["Categories"] = new MultiSelectList(categories, "Id", "Title");
            return View(product);
        }

        // GET: Blog/Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Blog/Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'AppDbContext.Posts'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            StatusMessage = $"Bạn vừa xoá bài viết: {product.Title}";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
          return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
