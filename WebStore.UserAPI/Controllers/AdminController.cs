using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.UserAPI.Models;
using WebStore.UserApp.Interfaces.Services;
using WebStore.UserDomain.Entities;

namespace WebStore.UserApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("Admin-GetAll")]
    public async Task<ActionResult<IEnumerable<Admin>>> GetAll(CancellationToken ct)
    {
        var admins = await _adminService.GetAllAdminsAsync(ct);
        return Ok(admins);
    }

    [HttpGet("Admin-GetById/{id:int}")]
    public async Task<ActionResult<Admin>> GetById(int id, CancellationToken ct)
    {
        try
        {
            var admin = await _adminService.GetAdminByIdAsync(id, ct);
            return Ok(admin);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize]
    [HttpPost("Admin-Register")]
    public async Task<ActionResult<int>> Register([FromBody] RegisterAdminRequest request, CancellationToken ct)
    {
        var id = await _adminService.RegisterAdminAsync(
            request.Username,
            request.Email,
            request.Password,
            ct);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [Authorize]
    [HttpPatch("Admin-ChangePassword/{id:int}")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        await _adminService.ChangePasswordAsync(id, request.OldPassword, request.NewPassword, ct);
        return NoContent();
    }

    [Authorize]
    [HttpPatch("Admin-ChangeUsername/{id:int}")]
    public async Task<IActionResult> UpdateUsername(int id, [FromBody] string newUsername, CancellationToken ct)
    {
        await _adminService.UpdateUsernameAsync(id, newUsername, ct);
        return NoContent();
    }

    [Authorize]
    [HttpPatch("Admin-ChangeEmail/{id:int}")]
    public async Task<IActionResult> UpdateEmail(int id, [FromBody] string newEmail, CancellationToken ct)
    {
        await _adminService.UpdateEmailAsync(id, newEmail, ct);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _adminService.RemoveAdminAsync(id, ct);
        return NoContent();
    }
}
