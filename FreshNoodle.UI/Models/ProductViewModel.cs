using System.ComponentModel.DataAnnotations;

namespace FreshNoodle.UI.Models;

public class ProductViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
