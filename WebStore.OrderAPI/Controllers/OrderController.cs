using Microsoft.AspNetCore.Mvc;
using WebStore.OrderApp.DTOs;
using WebStore.OrderApp.Interfaces.Services;

namespace WebStore.OrderAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("Create-Order/{customerId}")]
    public async Task<IActionResult> PlaceOrder(int customerId, CancellationToken ct)
    {
        var orderId = await _orderService.PlaceOrderAsync(customerId, ct);

        return CreatedAtAction(nameof(GetOrderById), new { customerId, orderId }, new { Id = orderId });
    }

    [HttpGet("Get-Orders/{customerId}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetMyOrders(int customerId, CancellationToken ct)
    {
        var orders = await _orderService.GetMyOrdersAsync(customerId, ct);
        return Ok(orders);
    }

    [HttpGet("Get-Order/{customerId}/{orderId}")]
    public async Task<ActionResult<Order>> GetOrderById(int customerId, int orderId, CancellationToken ct)
    {
        var order = await _orderService.GetMyOrderByIdAsync(customerId, orderId, ct);
        return Ok(order);
    }

    [HttpPost("Cancel-Order/{customerId}/{orderId}")]
    public async Task<IActionResult> CancelOrder(int customerId, int orderId, CancellationToken ct)
    {
        await _orderService.CancelOrderAsync(customerId, orderId, ct);
        return NoContent();
    }
}
