using Ecu911.BibliotecaService.DTOs;
using Ecu911.BibliotecaService.Interfaces;
using Ecu911.BibliotecaService.Models;

namespace Ecu911.BibliotecaService.Services;

public class BibliotecaDescargaService : IBibliotecaDescargaService
{
    private readonly IBibliotecaDescargaRepository _repository;
    private readonly AuditService _auditService;

    public BibliotecaDescargaService(
        IBibliotecaDescargaRepository repository,
        AuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
    }

    public async Task<List<BibliotecaDescargaDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<BibliotecaDescargaDto>> GetByBibliotecaDocumentoIdAsync(Guid documentItemId)
    {
        var items = await _repository.GetByBibliotecaDocumentoIdAsync(documentItemId);
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<BibliotecaDescargaDto>> GetByBibliotecaArchivoIdAsync(Guid documentFileId)
    {
        var items = await _repository.GetByBibliotecaArchivoIdAsync(documentFileId);
        return items.Select(MapToDto).ToList();
    }

    public async Task RegisterAsync(Guid documentItemId, Guid documentFileId, string? username)
    {
        var entity = new BibliotecaDescarga
        {
            BibliotecaDocumentoId = documentItemId,
            BibliotecaArchivoId = documentFileId,
            DownloadedAt = DateTime.UtcNow,
            DownloadedBy = username
        };

        await _repository.AddAsync(entity);

        _auditService.LogAction(
            "DownloadFile",
            username ?? "Unknown",
            $"Downloaded file for BibliotecaDocumento ID: {documentItemId}");
    }

    private static BibliotecaDescargaDto MapToDto(BibliotecaDescarga x)
    {
        return new BibliotecaDescargaDto
        {
            Id = x.Id,
            BibliotecaDocumentoId = x.BibliotecaDocumentoId,
            DocumentTitle = x.BibliotecaDocumento?.Title ?? "Desconocido",
            BibliotecaArchivoId = x.BibliotecaArchivoId,
            FileName = x.BibliotecaArchivo?.OriginalFileName ?? "Desconocido",
            DownloadedAt = x.DownloadedAt,
            DownloadedBy = x.DownloadedBy
        };
    }
}