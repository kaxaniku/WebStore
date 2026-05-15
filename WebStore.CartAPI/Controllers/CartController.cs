using Microsoft.AspNetCore.Mvc;
using WebStore.CartAPI.Models;
using WebStore.CartApp.Interfaces.Services;
using WebStore.CartDomain.Entities;

namespace WebStore.CartAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartsController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("Get-Cart/{customerId}")]
    public async Task<ActionResult<Cart>> Get(int customerId, CancellationToken ct)
    {
        var cart = await _cartService.GetCartAsync(customerId, ct);
        return Ok(cart);
    }

    [HttpPost("Add-To-Cart/{customerId}")]
    public async Task<IActionResult> AddItem(int customerId, [FromBody] AddToCartRequest request, CancellationToken ct)
    {
        await _cartService.AddToCartAsync(customerId, request.ProductId, request.Quantity, ct);
        return NoContent();
    }

    [HttpDelete("Remove-From-Cart/{customerId}/items/{productId}")]
    public async Task<IActionResult> RemoveItem(int customerId, int productId, CancellationToken ct)
    {
        await _cartService.RemoveFromCartAsync(customerId, productId, ct);
        return NoContent();
    }

    [HttpPatch("Increase-Quantity/{customerId}/items/{productId}")]
    public async Task<IActionResult> IncreaseQuantity(int customerId, int productId, [FromQuery] int quantity, CancellationToken ct)
    {
        await _cartService.IncreaseCartItemQuantityAsync(customerId, productId, quantity, ct);
        return NoContent();
    }

    [HttpPatch("Decrease-Quantity/{customerId}/items/{productId}")]
    public async Task<IActionResult> DecreaseQuantity(int customerId, int productId, [FromQuery] int quantity, CancellationToken ct)
    {
        await _cartService.DecreaseCartItemQuantityAsync(customerId, productId, quantity, ct);
        return NoContent();
    }

    [HttpDelete("Clear-Cart/{customerId}/items")]
    public async Task<IActionResult> Clear(int customerId, CancellationToken ct)
    {
        await _cartService.ClearCartAsync(customerId, ct);
        return NoContent();
    }

    [HttpGet("Get-Total/{customerId}")]
    public async Task<ActionResult<decimal>> GetTotal(int customerId, CancellationToken ct)
    {
        var total = await _cartService.GetCartTotalAsync(customerId, ct);
        return Ok(total);
    }
}
