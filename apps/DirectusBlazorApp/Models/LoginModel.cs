using System.ComponentModel.DataAnnotations;

namespace DirectusBlazorApp.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Password is required")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Password cannot be empty or whitespace")]
    public string Password { get; set; } = string.Empty;
}
