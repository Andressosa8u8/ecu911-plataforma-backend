using Ecu911.AuthService.DTOs;
using Ecu911.AuthService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecu911.AuthService.Controllers;

[Authorize(Roles = "ADMIN")]
[ApiController]
[Route("api/[controller]")]
public class SystemModulesController : ControllerBase
{
    private readonly IAuthService _authService;

    public SystemModulesController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _authService.GetSystemModulesAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSystemModuleDto input)
    {
        var result = await _authService.CreateSystemModuleAsync(input);
        return Ok(result);
    }

    [HttpPut("{systemModuleId:guid}")]
    public async Task<IActionResult> Update(Guid systemModuleId, [FromBody] UpdateSystemModuleDto input)
    {
        var result = await _authService.UpdateSystemModuleAsync(systemModuleId, input);

        if (result == null)
            return NotFound("Sistema no encontrado.");

        return Ok(result);
    }

    [HttpPatch("{systemModuleId:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid systemModuleId, [FromBody] ChangeSystemModuleStatusDto input)
    {
        var result = await _authService.ChangeSystemModuleStatusAsync(systemModuleId, input.IsActive);

        if (result == null)
            return NotFound("Sistema no encontrado.");

        return Ok(result);
    }

    [HttpPost("assign-user-role")]
    public async Task<IActionResult> AssignUserRole([FromBody] AssignUserSystemRoleDto input)
    {
        await _authService.AssignUserSystemRoleAsync(input);
        return Ok(new { message = "Rol asignado al usuario para el sistema correctamente." });
    }

    [HttpPost("remove-user-role")]
    public async Task<IActionResult> RemoveUserRoleFromSystem([FromBody] RemoveUserSystemRoleDto input)
    {
        await _authService.RemoveUserSystemRoleAsync(input);
        return Ok(new { message = "Rol removido del usuario en el sistema correctamente." });
    }

    [HttpPost("remove-user-scope")]
    public async Task<IActionResult> RemoveUserScopeFromSystem([FromBody] RemoveUserSystemScopeDto input)
    {
        await _authService.RemoveUserSystemScopeAsync(input);
        return Ok(new { message = "Alcance removido del usuario en el sistema correctamente." });
    }
}