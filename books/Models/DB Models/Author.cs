using System.ComponentModel.DataAnnotations;

namespace books.Models;

public class Author
{
    [Key]
    public int Id { get; set; }
    [Display(Name ="Author")]

    public string Name { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public List<Book>? Books { get; set; }

    public override string ToString() => $"{Name} {Surname}";
}