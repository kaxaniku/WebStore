using MapsterMapper;
using MassTransit;
using WebStore.CatalogApp.Interfaces.Repositories;
using WebStore.CatalogApp.Interfaces.Services;
using WebStore.CatalogDomain.Entities;
using WebStore.Contracts.Catalog.Category;

namespace WebStore.CatalogApp.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IProductService _productService;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IPublishEndpoint publishEndpoint, IProductService productService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct)
    {
        var categories = await _unitOfWork.CategoryRepository.QueryAsync(x => x.Activity.IsActive, ct);
        return _mapper.Map<IEnumerable<Category>>(categories);
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken ct)
    {
        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id, ct);
        if (category == null || !category.Activity.IsActive)
            return null;
        return _mapper.Map<Category>(category);
    }

    public async Task<int> AddAsync(string catName, CancellationToken ct)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            var categoryEntity = Category.Create(catName);
            var dto = _mapper.Map<DTOs.Category>(categoryEntity);
            await _unitOfWork.CategoryRepository.InsertAsync(dto, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            await _publishEndpoint.Publish(new CategoryCreated(dto.Id, catName), ct);
            await _unitOfWork.SaveChangesAsync(ct);
            Category.SetId(categoryEntity, dto.Id);
            await _unitOfWork.CommitAsync(ct);
            return dto.Id;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    public async Task UpdateAsync(int id, string newName, CancellationToken ct)
    {
        var dto = await _unitOfWork.CategoryRepository.GetByIdAsync(id, ct);
        if (dto == null)
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        var categoryEntity = _mapper.Map<Category>(dto);
        Category.UpdateName(categoryEntity, newName);
        _mapper.Map(categoryEntity, dto);
        await _unitOfWork.CategoryRepository.UpdateAsync(dto);
        await _publishEndpoint.Publish(new CategoryUpdated(dto.Id, newName), ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(int id, CancellationToken ct)
    {
        var dto = await _unitOfWork.CategoryRepository.GetByIdAsync(id, ct);
        if (dto == null)
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        _unitOfWork.CategoryRepository.Delete(dto);
        await _productService.RemoveProductsByCategoryIdAsync(id, ct);
        await _publishEndpoint.Publish(new CategoryDeleted(dto.Id), ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}