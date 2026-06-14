using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebStore.UserAPI.Models;
using WebStore.UserApp.Interfaces.Services;
using WebStore.UserDomain.Entities;

namespace WebStore.UserApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _config;

    public AuthController(IAuthService authService, IConfiguration config)
    {
        _authService = authService;
        _config = config;
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
    [HttpPost("login")]
    public async Task<IActionResult> Auth([FromBody] LoginRequest login, CancellationToken cancellationToken)
    {
        IActionResult response = Unauthorized();
        var user = await _authService.AdminLogin(login.Username, login.Password, cancellationToken);

        if (user != null)
        {
            var tokenString = GenerateJwtToken(user);
            response = Ok(new { Token = tokenString });
        }

        return response;
    }

    private string GenerateJwtToken(Admin login)
    {
        var jwtConfig = _config.GetSection("JwtConfig");

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, login.Username)
    };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwtConfig["Issuer"],
            audience: jwtConfig["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}