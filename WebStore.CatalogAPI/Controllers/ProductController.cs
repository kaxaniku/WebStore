using Microsoft.AspNetCore.Mvc;
using WebStore.CatalogAPI.Models;
using WebStore.CatalogApp.Interfaces.Services;
using WebStore.CatalogDomain.Entities;

namespace WebStore.CatalogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("Get-All-Products")]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll(CancellationToken ct)
    {
        var products = await _productService.GetAllProductsAsync(ct);
        return Ok(products);
    }

    [HttpGet("Get-Product-By-Id/{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id, CancellationToken ct)
    {
        var product = await _productService.GetProductByIdAsync(id, ct);
        if (product == null) return NotFound();

        return Ok(product);
    }

    [HttpPost("Create-Product")]
    public async Task<ActionResult<int>> Create([FromForm] ProductModel request, CancellationToken ct)
    {
        var id = await _productService.CreateProductAsync(
            request.Name,
            request.Price,
            request.Description,
            request.Stock,
            request.CategoryId,
            request.ImageFile!,
            ct);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("Update-Product/{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdate request, CancellationToken ct)
    {
        await _productService.UpdateProductAsync(id, request.Name, request.Description, ct);
        return NoContent();
    }

    [HttpDelete("Delete-Product/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _productService.DeleteProductAsync(id, ct);
        return NoContent();
    }

    [HttpGet("Search-Products")]
    public async Task<ActionResult<IEnumerable<Product>>> Search([FromQuery] string name, CancellationToken ct)
    {
        var products = await _productService.SearchProductsAsync(name, ct);
        return Ok(products);
    }

    [HttpGet("Get-Products-By-Category/{categoryId:int}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetByCategory(int categoryId, CancellationToken ct)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId, ct);
        return Ok(products);
    }

    [HttpPatch("Update-Product-Stock/{id:int}")]
    public async Task<IActionResult> UpdateStock(int id, int newStock, CancellationToken ct)
    {
        await _productService.UpdateProductStockAsync(id, newStock, ct);
        return NoContent();
    }

    [HttpPatch("Update-Product-Price/{id:int}")]
    public async Task<IActionResult> UpdatePrice(int id, decimal newPrice, CancellationToken ct)
    {
        await _productService.UpdateProductPriceAsync(id, newPrice, ct);
        return NoContent();
    }

    [HttpPatch("Update-Product-Image/{id:int}")]
    public async Task<IActionResult> UpdateImage(int id, IFormFile imageFile, CancellationToken ct)
    {
        await _productService.UpdateProductImageAsync(id, imageFile, ct);
        return NoContent();
    }
}
