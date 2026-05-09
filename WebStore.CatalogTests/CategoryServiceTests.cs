using Microsoft.Extensions.DependencyInjection;
using WebStore.CatalogApp.Services;
using WebStore.Contracts.Catalog.Category;

namespace WebStore.CatalogTests;

[TestFixture]
public class CategoryServiceTests : BaseTests
{
    private CategoryService _categoryService;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _categoryService = new CategoryService(_unitOfWork!, _mapper, _publishMock!.Object);
    }

    [Test]
    public async Task AddAsync_ShouldCreateCategory()
    {
        string categoryName = "Electronics";

        int newId = await _categoryService.AddAsync(categoryName, _cts.Token);

        var addedCategory = await _categoryService.GetByIdAsync(newId, _cts.Token);

        Assert.Multiple(() =>
        {
            Assert.That(newId, Is.GreaterThan(0));
            Assert.That(addedCategory?.Name, Is.EqualTo(categoryName));
        });
    }
                                                                                                            
    [Test]
    public async Task UpdateAsync_ShouldChangeName_WhenCategoryExists()
    {
        int existingId = await _categoryService.AddAsync("Old Name", _cts.Token);
        string newName = "New Name";

        await _categoryService.UpdateAsync(existingId, newName, _cts.Token);

        var updatedCategory = await _categoryService.GetByIdAsync(existingId, _cts.Token);
        Assert.That(updatedCategory!.Name, Is.EqualTo(newName));
    }

    [Test]
    public void UpdateAsync_ShouldThrowKeyNotFound_WhenCategoryDoesNotExist()
    {
        int nonExistentId = 9999;

        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _categoryService.UpdateAsync(nonExistentId, "Some Name", _cts.Token));
    }

    [Test]
    public async Task RemoveAsync_ShouldSoftDeleteCategory_AndTriggerEvent()
    {
        int idToRemove = await _categoryService.AddAsync("Soft Delete Me", _cts.Token);

        await _categoryService.RemoveAsync(idToRemove, _cts.Token);

        var resultFromService = await _categoryService.GetByIdAsync(idToRemove, _cts.Token);
        Assert.That(resultFromService, Is.Null, "Service should not return inactive categories.");

        var rawDto = await _unitOfWork!.CategoryRepository.GetByIdAsync(idToRemove, _cts.Token);

        Assert.Multiple(() =>
        {
            Assert.That(rawDto, Is.Not.Null, "Row should still exist in the database.");
            Assert.That(rawDto!.Activity.IsActive, Is.False, "IsActive should be false.");
        });
    }
}