using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models.Product
{
    public class ProductCategoryProduct
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey("ProductId")]
        public virtual ProductModel Product { get; set; } = default!;

        [ForeignKey("CategoryId")]
        public virtual CategoryProduct Category { get; set; } = default!;
        
    }
}
