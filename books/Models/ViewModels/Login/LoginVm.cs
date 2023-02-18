using System.ComponentModel.DataAnnotations;

namespace books.Models
{
    public class LoginVm
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; } = default!;
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = default!;
        [Display(Name = "Remember?")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
