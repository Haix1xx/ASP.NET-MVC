
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models.Blog
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [Required(ErrorMessage = "Phải có tiêu đề bài viết")]
        [Display(Name = "Tiêu đề")]
        [StringLength(160, MinimumLength = 5, ErrorMessage = "{0} dài {1} đến {2}")]
        public string Title { set; get; } = default!;
        [Display(Name = "Mô tả ngắn")]
        public string? Description { set; get; }

        [Display(Name = "Chuỗi định danh (url)", Prompt = "Nhập hoặc để trống tự phát sinh theo Title")]
        [Required(ErrorMessage = "Phải thiết lập chuỗi URL")]
        [StringLength(160, MinimumLength = 5, ErrorMessage = "{0} dài {1} đến {2}")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
        public string Slug { set; get; } = default!;

        [Display(Name = "Nội dung")]
        public string Content { set; get; } = default!;

        [Display(Name = "Xuất bản")]
        public bool Published { set; get; }

        public virtual IList<PostCategory> PostCategories { get; set; } = default!;

        [Required]
        [Display(Name = "Tác giả")]
        public string AuthorId { set; get; } = default!;
        [ForeignKey("AuthorId")]
        [Display(Name = "Tác giả")]
        public virtual AppUser Author { set; get; } = default!;

        [Display(Name = "Ngày tạo")]
        public DateTime DateCreated { set; get; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime DateUpdated { set; get; }
    }
}
