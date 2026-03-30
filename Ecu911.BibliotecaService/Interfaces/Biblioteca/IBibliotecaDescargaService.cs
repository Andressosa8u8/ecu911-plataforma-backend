using Ecu911.BibliotecaService.DTOs;

namespace Ecu911.BibliotecaService.Interfaces;

public interface IBibliotecaDescargaService
{
    Task<List<BibliotecaDescargaDto>> GetAllAsync();
    Task<List<BibliotecaDescargaDto>> GetByBibliotecaDocumentoIdAsync(Guid documentItemId);
    Task<List<BibliotecaDescargaDto>> GetByBibliotecaArchivoIdAsync(Guid documentFileId);
    Task RegisterAsync(Guid documentItemId, Guid documentFileId, string? username);
}