using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebStore.CatalogAPI.Models;
using WebStore.CatalogApp.Interfaces.Services;
using WebStore.CatalogDomain.Entities;

namespace WebStore.CatalogAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IAuthService _loginService;
    public AuthController(IConfiguration config, IAuthService loginService)
    {
        _config = config;
        _loginService = loginService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Auth([FromBody] UserLoginRequest login, CancellationToken cancellationToken)
    {
        IActionResult response = Unauthorized();
        var user = await _loginService.AdminLogin(login.Username, login.Password, cancellationToken);

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
