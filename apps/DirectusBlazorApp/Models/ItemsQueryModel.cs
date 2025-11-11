using System.ComponentModel.DataAnnotations;

namespace DirectusBlazorApp.Models;

public class ItemsQueryModel
{
    [Required(ErrorMessage = "Collection name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Collection name must be between 1 and 100 characters")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Collection name cannot be empty or whitespace")]
    public string CollectionName { get; set; } = "articles";

    [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
    public int Limit { get; set; } = 10;
}
