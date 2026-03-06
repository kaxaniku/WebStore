using System.ComponentModel.DataAnnotations;

namespace WebStore.Application.DTOs;

public record Cart
{
    [Key]
    public int Id;

    public User User = null!;

    public ICollection<Product>? Products { get; set; }
}
