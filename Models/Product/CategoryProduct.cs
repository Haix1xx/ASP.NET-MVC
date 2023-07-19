using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models.Product
{
    [Table("CategoryProduct")]
    public class CategoryProduct
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Danh mục cha")]
        public int? ParentCategoryId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "{0} phải dài từ {1} đến {2}")]
        [Display(Name = "Tên danh mục")]
        public string Title { get; set; } = default!;

        [DataType(DataType.Text)]
        [Display(Name = "Nội dung danh mục")]
        public string Description { get; set; } = default!;

        //chuỗi Url
        [Required(ErrorMessage = "Phải tạo url")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
        [Display(Name = "Url hiện thị")]
        public string Slug { set; get; } = default!;

        // Các Category con
        public ICollection<CategoryProduct>? CategoryChildren { get; set; }

        [ForeignKey("ParentCategoryId")]
        [Display(Name = "Danh mục cha")]

        public CategoryProduct? ParentCategory { set; get; }

        public void ChildCategoryIds(List<int> lists, ICollection<CategoryProduct>? childcates = null)
        {
            if (childcates == null)
                return;
            foreach (var category in childcates)
            {
                lists.Add(category.Id);
                ChildCategoryIds(lists, category?.CategoryChildren);

            }
        }

        public IEnumerable<CategoryProduct>? GetParents()
        {
            Stack<CategoryProduct> parents = new Stack<CategoryProduct>();
            var tmp = ParentCategory;
            while(tmp != null)
            {
                parents.Push(tmp);
                tmp = tmp.ParentCategory;
            }
            return parents;
        }
    }
}
