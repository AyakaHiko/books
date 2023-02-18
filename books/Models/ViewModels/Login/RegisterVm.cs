using System.ComponentModel.DataAnnotations;

namespace books.Models;

public class RegisterVm
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = default!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords are different")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = default!;
}