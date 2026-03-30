using Ecu911.BibliotecaService.DTOs;
using Ecu911.BibliotecaService.Helpers;
using Ecu911.BibliotecaService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecu911.BibliotecaService.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BibliotecaCategoriasController : ControllerBase
{
    private readonly IBibliotecaCategoriaService _service;

    public BibliotecaCategoriasController(IBibliotecaCategoriaService service)
    {
        _service = service;
    }

    [Authorize(Roles = "ADMIN,CONSULTA,GESTOR_DOCUMENTAL")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,GESTOR_DOCUMENTAL")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBibliotecaCategoriaDto input)
    {
        var username = UserContextHelper.GetUsername(User);
        var result = await _service.CreateAsync(input, username);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,CONSULTA,GESTOR_DOCUMENTAL")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "Tipo de documento no encontrado." });

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBibliotecaCategoriaDto input)
    {
        var username = UserContextHelper.GetUsername(User);
        var result = await _service.UpdateAsync(id, input, username);

        if (result == null)
            return NotFound(new { message = "Tipo de documento no encontrado." });

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var username = UserContextHelper.GetUsername(User);
        var result = await _service.ActivateAsync(id, username);

        if (result == null)
            return NotFound(new { message = "Tipo de documento no encontrado." });

        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var username = UserContextHelper.GetUsername(User);
        var deleted = await _service.DeleteAsync(id, username);

        if (!deleted)
            return NotFound(new { message = "Tipo de documento no encontrado." });

        return Ok(new { message = "Tipo de documento desactivado correctamente." });
    }
}