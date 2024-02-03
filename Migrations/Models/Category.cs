#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XXL_Market.Models;
namespace XXL_Market.Models;
public class Category
{
    public int CategoryId { get; set; }
    [ Required(ErrorMessage = "Category name is required")]
    [MinLength(3, ErrorMessage = "Category name must be at least 3 characters")]
    public string Name { get; set; }
    public List<Product> Products { get; set; } = new List<Product>();
}
