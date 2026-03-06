using System.ComponentModel.DataAnnotations;
using WebStore.Domain;

namespace WebStore.Application.DTOs;

public sealed class Cart
{
    [Key]
    public int Id { get; set; }

    public User User { get; set; } = null!;

    public ActivityInfo Activity { get; set; } = null!;
}
