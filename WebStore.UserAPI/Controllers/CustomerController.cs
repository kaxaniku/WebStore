using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.UserAPI.Models;
using WebStore.UserApp.Interfaces.Services;
using WebStore.UserDomain.Entities;

namespace WebStore.UserApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet("Customer-GetAll")]
    public async Task<ActionResult<IEnumerable<Customer>>> GetAll(CancellationToken ct)
    {
        var customers = await _customerService.GetAllCustomersAsync(ct);
        return Ok(customers);
    }

    [HttpGet("Customer-GetById/{id:int}")]
    public async Task<ActionResult<Customer>> GetById(int id, CancellationToken ct)
    {
        try
        {
            var customer = await _customerService.GetCustomerByIdAsync(id, ct);
            return Ok(customer);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("Customer-Register")]
    public async Task<ActionResult<int>> Register([FromBody] RegisterCustomerRequest request, CancellationToken ct)
    {
        var id = await _customerService.RegisterCustomerAsync(
            request.Username,
            request.Email,
            request.Password,
            ct);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPatch("Customer-ChangePassword/{id:int}")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        await _customerService.ChangePasswordAsync(id, request.OldPassword, request.NewPassword, ct);
        return NoContent();
    }

    [HttpPatch("Customer-ChangeUsername/{id:int}")]
    public async Task<IActionResult> UpdateUsername(int id, [FromBody] string newUsername, CancellationToken ct)
    {
        await _customerService.UpdateUsernameAsync(id, newUsername, ct);
        return NoContent();
    }

    [HttpPatch("Customer-ChangeEmail/{id:int}")]
    public async Task<IActionResult> UpdateEmail(int id, [FromBody] string newEmail, CancellationToken ct)
    {
        await _customerService.UpdateEmailAsync(id, newEmail, ct);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _customerService.RemoveCustomerAsync(id, ct);
        return NoContent();
    }
}
