using Ecu911.BibliotecaService.DTOs;

namespace Ecu911.BibliotecaService.Interfaces;

public interface IBibliotecaArchivoService
{
    Task<BibliotecaArchivoDto> UploadAsync(Guid documentItemId, IFormFile file, string? username, bool isAdmin, Guid? organizationalUnitId, CancellationToken cancellationToken = default);
    Task<BibliotecaArchivoDto?> GetMetadataAsync(Guid documentItemId, bool isAdmin, Guid? organizationalUnitId, CancellationToken cancellationToken = default);
    Task<BibliotecaArchivoDownloadDto?> DownloadAsync(Guid documentItemId, bool isAdmin, Guid? organizationalUnitId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid documentItemId, string? username, bool isAdmin, Guid? organizationalUnitId, CancellationToken cancellationToken = default);
}