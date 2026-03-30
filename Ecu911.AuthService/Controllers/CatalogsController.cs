using Ecu911.AuthService.Data;
using Ecu911.AuthService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.AuthService.Controllers;

[ApiController]
[Route("api/catalogos")]
[Authorize]
public class CatalogsController : ControllerBase
{
    private readonly AuthDbContext _context;

    public CatalogsController(AuthDbContext context)
    {
        _context = context;
    }

    [HttpGet("provincias")]
    public async Task<ActionResult<List<ProvinceDto>>> GetProvincias()
    {
        var result = await _context.Provinces
            .Where(x => x.IsActive)
            .OrderBy(x => x.Nombre)
            .Select(x => new ProvinceDto
            {
                Id = x.Id,
                Nombre = x.Nombre
            })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("cantones")]
    public async Task<ActionResult<List<CantonDto>>> GetCantones([FromQuery] int provinciaId)
    {
        var query = _context.Cantons.Where(x => x.IsActive);
        if (provinciaId > 0)
            query = query.Where(x => x.ProvinciaId == provinciaId);

        var result = await query
            .OrderBy(x => x.Nombre)
            .Select(x => new CantonDto
            {
                Id = x.Id,
                ProvinciaId = x.ProvinciaId,
                Nombre = x.Nombre
            })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("centros-zonales")]
    public async Task<ActionResult<List<CentroZonalDto>>> GetCentros([FromQuery] int provinciaId)
    {
        var query = _context.CentrosZonales.Where(x => x.IsActive);
        if (provinciaId > 0)
            query = query.Where(x => x.ProvinciaId == provinciaId);

        var result = await query
            .OrderBy(x => x.Nombre)
            .Select(x => new CentroZonalDto
            {
                Id = x.Id,
                ProvinciaId = x.ProvinciaId,
                Nombre = x.Nombre,
                Sigla = x.Sigla,
                Grupo = x.Grupo,
                ParentId = x.ParentId
            })
            .ToListAsync();

        return Ok(result);
    }
}
