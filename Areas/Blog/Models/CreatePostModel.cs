using MVC.Models.Blog;
using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Blog.Models
{
    public class CreatePostModel : Post
    {
        [Display(Name = "Chuyên mục")]
        public int[]? CategoryIDs { get; set; }
        public CreatePostModel()
        {

        }

        public CreatePostModel(Post post)
        {
            PostId = post.PostId;
            Title = post.Title;
            Description= post.Description;
            Content = post.Content;
            Slug = post.Slug;
            Published = post.Published;
            CategoryIDs = post.PostCategories.Select(pc => pc.CategoryId).ToArray();
        }
    }
}
