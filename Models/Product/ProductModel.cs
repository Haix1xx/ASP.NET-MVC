
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models.Product
{
    [Table("Product")]
    public class ProductModel
    {
        [Key]
        public int ProductId { get; set; }
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

        

        // [Required]
        [Display(Name = "Người đăng")]
        public string? AuthorId { set; get; }
        [ForeignKey("AuthorId")]
        [Display(Name = "Tác giả -")]
        public virtual AppUser? Author { set; get; }

        [Display(Name = "Ngày tạo")]
        public DateTime DateCreated { set; get; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime DateUpdated { set; get; }

        [Display(Name = "Giá sản phẩm")]
        [Range(0 , int.MaxValue, ErrorMessage = "Phải nhập giá trị từ {1}")]
        [Column(TypeName = "decimal(19,4)")]
        public decimal Price { set; get; }

        public virtual IList<ProductCategoryProduct>? ProductCategoryProducts { get; set; }
    }
}
