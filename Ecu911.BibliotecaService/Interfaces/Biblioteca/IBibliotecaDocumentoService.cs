using Ecu911.BibliotecaService.DTOs;

namespace Ecu911.BibliotecaService.Interfaces;

public interface IBibliotecaDocumentoService
{
    Task<PagedResultDto<BibliotecaDocumentoDto>> GetAllAsync(BibliotecaDocumentoFilterDto filter, int pageIndex, int pageSize, bool isAdmin, Guid? organizationalUnitId);
    Task<BibliotecaDocumentoDto?> GetByIdAsync(Guid id, bool isAdmin, Guid? organizationalUnitId);
    Task<BibliotecaDocumentoDto> CreateAsync(CreateBibliotecaDocumentoDto input, string? username, bool isAdmin, Guid? organizationalUnitId);
    Task<BibliotecaDocumentoDto?> UpdateAsync(Guid id, UpdateBibliotecaDocumentoDto input, string? username, bool isAdmin, Guid? organizationalUnitId);
    Task<bool> DeleteAsync(Guid id, string? username, bool isAdmin, Guid? organizationalUnitId);
}