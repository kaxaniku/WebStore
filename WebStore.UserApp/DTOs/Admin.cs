using WebStore.UserApp.Interfaces;

namespace WebStore.UserApp.DTOs;

public sealed class Admin : User, IDisable
{
    public ActivityInfo Activity { get; set; } = new();
}
