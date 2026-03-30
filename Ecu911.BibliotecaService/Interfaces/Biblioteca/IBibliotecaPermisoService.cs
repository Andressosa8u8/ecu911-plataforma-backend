using Ecu911.BibliotecaService.DTOs;

namespace Ecu911.BibliotecaService.Interfaces;

public interface IBibliotecaPermisoService
{
    Task<List<BibliotecaPermisoDto>> GetAllAsync();
    Task<List<BibliotecaPermisoDto>> GetByNodeIdAsync(Guid repositoryNodeId);
    Task<List<BibliotecaPermisoDto>> GetByOrganizationalUnitIdAsync(Guid organizationalUnitId);
    Task<BibliotecaPermisoDto?> GetByIdAsync(Guid id);
    Task<BibliotecaPermisoDto> CreateAsync(CreateBibliotecaPermisoDto input, string? username);
    Task<BibliotecaPermisoDto?> UpdateAsync(Guid id, UpdateBibliotecaPermisoDto input, string? username);
    Task<bool> DeleteAsync(Guid id, string? username);
}