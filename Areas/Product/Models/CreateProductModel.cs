using MVC.Models.Blog;
using MVC.Models.Product;
using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Product.Models;

public class CreateProductModel : ProductModel
{
    [Display(Name = "Chuyên mục")]
    public int[]? CategoryIDs { get; set; }
    public CreateProductModel()
    {

    }

    public CreateProductModel(ProductModel product)
    {
        ProductId = product.ProductId;
        Title = product.Title;
        Description= product.Description;
        Content = product.Content;
        Slug = product.Slug;
        Published = product.Published;
        CategoryIDs = product.ProductCategoryProducts.Select(pc => pc.CategoryId).ToArray();
        Price = product.Price;
    }
}
