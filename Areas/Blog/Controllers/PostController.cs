using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC.Areas.Blog.Models;
using MVC.Data;
using MVC.Models;
using MVC.Models.Blog;

namespace MVC.Areas.Blog.Controllers
{
    [Area("Blog")]
    [Route("admin/blog/post/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator + "," + RoleName.Editor)]
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public PostController(AppDbContext context, UserManager<AppUser> userManager)
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
            int totalPost = await _context.Posts.CountAsync();
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
            var posts = _context.Posts.Skip((currentPage - 1) * pageSize)
                                        .Take(pageSize)
                                        .Include(p => p.PostCategories)
                                        .ThenInclude(pc => pc.Category); 
            return View(await posts.ToListAsync());
        }

        // GET: Blog/Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Blog/Post/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories?.ToListAsync();
            ViewData["Categories"] = new MultiSelectList(categories, "Id", "Title");
            return View();
        }

        // POST: Blog/Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,Published,CategoryIDs")] CreatePostModel post)
        {
            if (await _context.Posts.AnyAsync(p => p.Slug == post.Slug))
            {
                ModelState.AddModelError("Slug", "Slug này đã được sử dụng");
                return View(post);
            }    
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                post.DateCreated = DateTime.Now;
                post.AuthorId = user.Id;
                _context.Add(post);
                if (post.CategoryIDs != null)
                {
                    foreach(var categoryId in post.CategoryIDs)
                    {
                        await _context.AddAsync(new PostCategory
                        {
                            CategoryId = categoryId,
                            Post = post
                        });
                    }    
                }
                StatusMessage = "Vừa tạo bài viết mới";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var categories = await _context.Categories?.ToListAsync();
            ViewData["Categories"] = new MultiSelectList(categories, "Id", "Title");
            return View(post);
        }

        // GET: Blog/Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.AsNoTracking().Include(p => p.PostCategories).AsNoTracking().FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
            {
                return NotFound();
            }
            var postEdit = new CreatePostModel(post);
            var categories = await _context.Categories?.ToListAsync();
            ViewData["Categories"] = new MultiSelectList(categories, "Id", "Title");
            return View(postEdit);
        }

        // POST: Blog/Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Description,Slug,Content,Published,CategoryIDs")] CreatePostModel post)
        {
            if (id != post.PostId)
            {
                return NotFound("1");
            }

            if (await _context.Posts.AnyAsync(p => p.Slug == post.Slug && p.PostId != id))
            {
                ModelState.AddModelError(string.Empty, "Nhập slug khác");
                return View(post);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var postUpdate = await _context.Posts.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.PostId == id);
                    if (postUpdate == null)
                        return NotFound("2");

                    postUpdate.Title = post.Title;
                    postUpdate.Description = post.Description;
                    postUpdate.Content = post.Content;
                    postUpdate.Published = post.Published;
                    postUpdate.Slug = post.Slug;
                    postUpdate.DateUpdated = DateTime.Now;

                    // cap nhat post categories
                    if (post.CategoryIDs == null)
                        post.CategoryIDs = new int[] { } ;

                    var oldCateIds = postUpdate.PostCategories.Select(pc => pc.PostId).ToArray();
                    var newCateIds = post.CategoryIDs;

                    var removeCatePosts = postUpdate.PostCategories.Where(p => !newCateIds.Contains(p.CategoryId));
                    //var removeCatePosts = from postCate in postUpdate.PostCategories
                    //                    where (!newCateIds.Contains(postCate.CategoryId))
                    //                    select postCate;
                    _context.PostCategories.RemoveRange(removeCatePosts);
                    //var addIds = from CateId in newCateIds
                    //             where (!oldCateIds.Contains(CateId))
                    //             select CateId;
                    var addIds = newCateIds.Where(p => !oldCateIds.Contains(p));

                    
                    foreach(var CateId in addIds)
                    {
                        _context.PostCategories.Add(new PostCategory
                        {
                            CategoryId = CateId,
                            PostId = id,
                        });
                    }
                    _context.Update(postUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
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
            var categories = await _context.Categories?.ToListAsync();
            ViewData["Categories"] = new MultiSelectList(categories, "Id", "Title");
            return View(post);
        }

        // GET: Blog/Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Blog/Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'AppDbContext.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            _context.Posts.Remove(post);
            StatusMessage = $"Bạn vừa xoá bài viết: {post.Title}";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
          return (_context.Posts?.Any(e => e.PostId == id)).GetValueOrDefault();
        }
    }
}
