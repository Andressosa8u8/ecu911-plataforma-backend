using Ecu911.AuthService.DTOs;
using Ecu911.AuthService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto input)
    {
        var result = await _authService.LoginAsync(input);

        if (result == null)
            return Unauthorized("Credenciales inválidas.");

        return Ok(result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(ClaimTypes.Name)
                         ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("No se pudo identificar al usuario autenticado.");

        var result = await _authService.GetCurrentUserAsync(userId);

        if (result == null)
            return NotFound("Usuario no encontrado.");

        result.CurrentSystem = User.FindFirstValue("system_code");

        return Ok(result);
    }

    [HttpPost("select-system")]
    public async Task<IActionResult> SelectSystem([FromBody] SelectSystemDto input)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(ClaimTypes.Name)
                         ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("No se pudo identificar al usuario autenticado.");

        var result = await _authService.SelectSystemAsync(userId, input);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _authService.GetUsersAsync();
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("users/{userId:guid}/access-profile")]
    public async Task<IActionResult> GetUserAccessProfile(Guid userId)
    {
        var result = await _authService.GetUserAccessProfileAsync(userId);

        if (result == null)
            return NotFound("Usuario no encontrado.");

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        var result = await _authService.GetRolesAsync();
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto input)
    {
        var result = await _authService.CreateUserAsync(input);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("users/{userId:guid}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto input)
    {
        var result = await _authService.UpdateUserAsync(userId, input);

        if (result == null)
            return NotFound("Usuario no encontrado.");

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto input)
    {
        var result = await _authService.CreateRoleAsync(input);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("roles/{roleId:guid}")]
    public async Task<IActionResult> UpdateRole(Guid roleId, [FromBody] UpdateRoleDto input)
    {
        var result = await _authService.UpdateRoleAsync(roleId, input);

        if (result == null)
            return NotFound("Rol no encontrado.");

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPatch("roles/{roleId:guid}/status")]
    public async Task<IActionResult> ChangeRoleStatus(Guid roleId, [FromBody] ChangeRoleStatusDto input)
    {
        var result = await _authService.ChangeRoleStatusAsync(roleId, input.IsActive);

        if (result == null)
            return NotFound("Rol no encontrado.");

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("users/{userId:guid}/roles")]
    public async Task<IActionResult> AssignRole(Guid userId, [FromBody] AssignRoleDto input)
    {
        await _authService.AssignRoleAsync(userId, input.RoleId);
        return Ok();
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("assign-system-scope")]
    public async Task<IActionResult> AssignSystemScope([FromBody] AssignUserSystemScopeDto input)
    {
        await _authService.AssignUserSystemScopeAsync(input);
        return Ok(new { message = "Alcance asignado correctamente." });
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("users/{userId:guid}/roles/{roleId:guid}")]
    public async Task<IActionResult> RemoveRole(Guid userId, Guid roleId)
    {
        await _authService.RemoveRoleAsync(userId, roleId);
        return Ok(new { message = "Rol global removido correctamente." });
    }

    [HttpGet("me/access-context")]
    public async Task<IActionResult> GetAccessContext()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(ClaimTypes.Name)
                         ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("No se pudo identificar al usuario autenticado.");

        var currentSystemCode = User.FindFirstValue("system_code");
        var context = await _authService.GetAccessContextAsync(userId, currentSystemCode);

        return Ok(context);
    }
}