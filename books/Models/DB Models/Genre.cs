using System.ComponentModel.DataAnnotations;

namespace books.Models;

public class Genre
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name ="Genre")]
    public string Name { get; set; } = default!;
    public List<Book>? Books { get; set; }

    public override string ToString()
        => Name;
}