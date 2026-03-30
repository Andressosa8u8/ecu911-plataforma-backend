using Ecu911.BibliotecaService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecu911.BibliotecaService.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BibliotecaDescargasController : ControllerBase
{
    private readonly IBibliotecaDescargaService _service;

    public BibliotecaDescargasController(IBibliotecaDescargaService service)
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

    [Authorize(Roles = "ADMIN,CONSULTA,GESTOR_DOCUMENTAL")]
    [HttpGet("by-document/{documentItemId:guid}")]
    public async Task<IActionResult> GetByDocument(Guid documentItemId)
    {
        var result = await _service.GetByBibliotecaDocumentoIdAsync(documentItemId);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,CONSULTA,GESTOR_DOCUMENTAL")]
    [HttpGet("by-file/{documentFileId:guid}")]
    public async Task<IActionResult> GetByFile(Guid documentFileId)
    {
        var result = await _service.GetByBibliotecaArchivoIdAsync(documentFileId);
        return Ok(result);
    }
}