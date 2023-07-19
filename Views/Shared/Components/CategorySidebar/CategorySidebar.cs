using Microsoft.AspNetCore.Mvc;
using MVC.Models.Blog;

namespace MVC.Views.Shared.Components.CategorySidebar
{

    public class CategorySidebar : ViewComponent
    {
        public class CategorySidebarData
        {
            public IEnumerable<Category> Categories { get; set; } = default!;
            public int Level { get; set; }
            public string CategorySlug { get; set; } = default!;
        }
        public IViewComponentResult Invoke(CategorySidebarData data)
        {
            return View(data);
        }
    }
}
