using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models.Blog
{
    public class PostCategory
    {
        public int PostId { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey("PostId")]
        public virtual Post Post { get; set; } = default!;

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = default!;
        
    }
}
