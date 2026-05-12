using WebStore.UserApp.Interfaces;

namespace WebStore.UserApp.DTOs;

public sealed class Customer : User, IDisable
{
    public ActivityInfo Activity { get; set; } = new();
}
