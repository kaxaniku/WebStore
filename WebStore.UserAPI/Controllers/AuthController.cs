using Microsoft.AspNetCore.Mvc;
using WebStore.UserAPI.Models;
using WebStore.UserApp.Interfaces.Services;
using WebStore.UserDomain.Entities;

namespace WebStore.UserApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("Customer-Login")]
    public async Task<ActionResult<Customer>> CustomerLogin([FromBody] LoginRequest request, CancellationToken ct)
    {
        try
        {
            var customer = await _authService.CustomerLogin(request.Username, request.Password, ct);

            if (customer == null)
            {
                return Unauthorized(new { message = "Invalid customer username or password." });
            }

            return Ok(customer);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("Admin-Login")]
    public async Task<ActionResult<Admin>> AdminLogin([FromBody] LoginRequest request, CancellationToken ct)
    {
        try
        {
            var admin = await _authService.AdminLogin(request.Username, request.Password, ct);

            if (admin == null)
            {
                return Unauthorized(new { message = "Invalid admin username or password." });
            }

            return Ok(admin);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}