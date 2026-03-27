using WebStore.Domain.ValueObjects;

namespace WebStore.Domain;
public sealed class Category
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    private Category()
    {
    }

    public static Category Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name name cannot be null or empty.", nameof(name));
        return new Category
        {
            Name = name,
        };
    }

    public static void SetId(Category category, int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be a positive integer.", nameof(id));
        category.Id = id;
    }

    public static void UpdateName(Category category, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name cannot be null or empty.", nameof(newName));
        category.Name = newName;
    }
}
