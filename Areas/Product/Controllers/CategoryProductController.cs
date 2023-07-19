using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;
using MVC.Models.Product;

namespace MVC.Areas.Product.Controllers
{
    [Area("Product")]
    [Route("admin/categoryproduct/category/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator)]
    public class CategoryProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CategoryProductController> _logger;

        public CategoryProductController(AppDbContext context, ILogger<CategoryProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Blog/Category
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.CategoryProducts
                                .Include(c => c.ParentCategory)
                                .Include(c => c.CategoryChildren);
            var result = (await appDbContext.ToListAsync()).Where(c => c.ParentCategory == null).ToList();
            return View(result);
        }

        // GET: Blog/Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CategoryProducts == null)
            {
                return NotFound();
            }

            var category = await _context.CategoryProducts
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Blog/Category/Create
        public async Task<IActionResult> Create()
        {
            var categories = new List<CategoryProduct>();
            categories.Add(new CategoryProduct
            {
                Id = -1,
                Title = "Trống"
            });
            categories.AddRange(await _context.CategoryProducts.ToListAsync());
            ViewData["ParentCategoryId"] = new SelectList(categories, "Id", "Title");
            return View();
        }

        // POST: Blog/Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParentCategoryId,Title,Description,Slug")] CategoryProduct category)
        {
            if (ModelState.IsValid)
            {
                if (category.ParentCategoryId == -1)
                    category.ParentCategoryId = null;
                _context.Add(category);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created a category successfully");
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentCategoryId"] = new SelectList(_context.CategoryProducts, "Id", "Slug", category.ParentCategoryId);
            _logger.LogInformation("Failed");
            return View(category);
        }

        // GET: Blog/Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CategoryProducts == null)
            {
                return NotFound();
            }

            var category = await _context.CategoryProducts.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var categories = new List<CategoryProduct>
            {
                new CategoryProduct
                {
                    Id = -1,
                    Title = "Trống"
                }
            };
            categories.AddRange(await _context.CategoryProducts.ToListAsync());
            var parentCategoyId = category?.ParentCategoryId ?? -1;
            ViewData["ParentCategoryId"] = new SelectList(categories, "Id", "Title", parentCategoyId);
            return View(category);
        }

        // POST: Blog/Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ParentCategoryId,Title,Description,Slug")] CategoryProduct category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }
            if (category.Id == category.ParentCategoryId)
            {
                ModelState.AddModelError(string.Empty, "Phải chọn danh mục cha khác");
            }    
            if (ModelState.IsValid && category.Id != category.ParentCategoryId)
            {
                try
                {
                    if (category.ParentCategoryId == -1)
                        category.ParentCategoryId = null;
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            var categories = new List<CategoryProduct>
            {
                new CategoryProduct
                {
                    Id = -1,
                    Title = "Trống"
                }
            };
            categories.AddRange(await _context.CategoryProducts.ToListAsync());
            var parentCategoyId = category?.ParentCategoryId ?? -1;
            ViewData["ParentCategoryId"] = new SelectList(categories, "Id", "Title", parentCategoyId);
            return View(category);
        }

        // GET: Blog/Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CategoryProducts == null)
            {
                return NotFound();
            }

            var category = await _context.CategoryProducts
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Blog/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CategoryProducts == null)
            {
                return Problem("Entity set 'AppDbContext.Categories'  is null.");
            }
            var category = await _context.CategoryProducts.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (category != null)
            {
                // Explicit loading
                foreach(var childCategory in await _context.Entry(category).Collection(c => c.CategoryChildren).Query().ToListAsync())
                {
                    childCategory.ParentCategoryId = category?.ParentCategoryId ?? null;
                }
                _context.CategoryProducts.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
