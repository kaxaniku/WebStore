using Microsoft.Extensions.DependencyInjection;
using WebStore.CatalogApp.Services;
using WebStore.Contracts.Catalog.Category;

namespace WebStore.CatalogTests;

[TestFixture]
public class CategoryServiceTests : BaseTests
{
    private CategoryService _categoryService;

    [SetUp]
    public async override Task SetUp()
    {
        await base.SetUp();

        _categoryService = _harness.Scope.ServiceProvider.GetRequiredService<CategoryService>();
    }

    [Test]
    public async Task AddAsync_ShouldCreateCategory_AndTriggerEvent()
    {
        string categoryName = "Electronics";

        int newId = await _categoryService.AddAsync(categoryName, _cts.Token);

        var AddedCategory = await _categoryService.GetByIdAsync(newId, _cts.Token);

        _cts.CancelAfter(TimeSpan.FromSeconds(5));

        Assert.Multiple(async () =>
        {
            Assert.That(newId, Is.GreaterThan(0));
            Assert.That(AddedCategory?.Name, Is.EqualTo(categoryName));

            var published = await _harness.Published.Any<CategoryCreated>(
                x => x.Context!.Message.Name == categoryName,
                _cts.Token
            );
            Assert.That(published, Is.True, "Message did not reach the bus.");
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

        var wasUpdatedPublished = await _harness.Published.Any<CategoryUpdated>(m =>
            m.Context!.Message.Id == existingId &&
            m.Context!.Message.Name == newName);

        Assert.That(wasUpdatedPublished, Is.True, "CategoryUpdated message was not found in the harness.");

        Assert.That(GetOutboxMessageCount(), Is.EqualTo(2), "Both Add and Update should be in the outbox.");
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

        Assert.Multiple(async () =>
        {
            Assert.That(rawDto, Is.Not.Null, "Row should still exist in the database.");
            Assert.That(rawDto!.Activity.IsActive, Is.False, "IsActive should be false.");

            var wasDeletedPublished = await _harness.Published.Any<CategoryDeleted>(m =>
                m.Context!.Message.Id == idToRemove);

            Assert.That(wasDeletedPublished, Is.True, "CategoryDeleted message was not found in the harness.");
        });
    }
}