using MapsterMapper;
using WebStore.CatalogApp.Interfaces.Repositories;
using WebStore.CatalogApp.Interfaces.Services;
using WebStore.CatalogDomain.Entities;

namespace WebStore.CatalogApp.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public static event Action<Category>? CategoryAdded;
    public static event Action<Category>? CategoryUpdated;
    public static event Action<int>? CategoryRemoved;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.CategoryRepository.QueryAsync(x => x.Activity.IsActive, cancellationToken);
        return _mapper.Map<IEnumerable<Category>>(categories);
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null || !category.Activity.IsActive)
            return null;
        return _mapper.Map<Category>(category);
    }

    public async Task<int> AddAsync(string catName, CancellationToken cancellationToken)
    {
        var categoryEntity = Category.Create(catName);
        var dto = _mapper.Map<DTOs.Category>(categoryEntity);
        await _unitOfWork.CategoryRepository.InsertAsync(dto, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        Category.SetId(categoryEntity, dto.Id);
        OnCategoryAdded(categoryEntity);
        return dto.Id;
    }

    public async Task UpdateAsync(int id, string newName, CancellationToken cancellationToken)
    {
        var dto = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);
        if (dto == null)
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        var categoryEntity = _mapper.Map<Category>(dto);
        Category.UpdateName(categoryEntity, newName);
        _mapper.Map(categoryEntity, dto);
        await _unitOfWork.CategoryRepository.UpdateAsync(dto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        OnCategoryUpdated(categoryEntity);
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken)
    {
        var dto = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);
        if (dto == null)
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        _unitOfWork.CategoryRepository.Delete(dto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        OnCategoryRemoved(id);
    }

    private static void OnCategoryAdded(Category customer)
    {
        CategoryAdded?.Invoke(customer);
    }

    private static void OnCategoryUpdated(Category customer)
    {
        CategoryUpdated?.Invoke(customer);
    }

    private static void OnCategoryRemoved(int customerId)
    {
        CategoryRemoved?.Invoke(customerId);
    }
}