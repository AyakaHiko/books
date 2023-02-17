using System.ComponentModel.DataAnnotations;

namespace books.Models
{
    public class LoginVm
    {
        [Required]
        public string Email { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
    }
}
