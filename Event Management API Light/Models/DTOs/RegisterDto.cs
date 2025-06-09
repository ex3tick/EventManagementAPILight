using System.ComponentModel.DataAnnotations;

namespace Event_Management_API_Light.Models.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(12, ErrorMessage = "Password must be at least 12 characters long")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

}