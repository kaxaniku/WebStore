using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebStore.Domain;

namespace WebStore.Application.DTOs;

public enum Role : byte
{
    Owner = 0,
    Admin = 1,
    User = 2
}

public sealed class User
{
    [Key]
    public int Id { get; set; }

    [MaxLength(20)]
    [Column(TypeName = "VARCHAR")]
    public string Username { get; set; } = null!;

    [MaxLength(50)]
    [Column(TypeName = "VARCHAR")]
    public string Email { get; set; } = null!;

    [MaxLength(100)]
    public string PasswordHash { get; set; } = null!;

    public Role Role { get; set; } = Role.User;

    public ActivityInfo Activity { get; set; } = null!;
}
