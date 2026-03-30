using Ecu911.BibliotecaService.Models;

namespace Ecu911.BibliotecaService.Interfaces;

public interface IBibliotecaPermisoRepository
{
    Task<List<BibliotecaPermiso>> GetAllAsync();
    Task<List<BibliotecaPermiso>> GetByNodeIdAsync(Guid repositoryNodeId);
    Task<List<BibliotecaPermiso>> GetByOrganizationalUnitIdAsync(Guid organizationalUnitId);
    Task<BibliotecaPermiso?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid repositoryNodeId, Guid organizationalUnitId, Guid? excludeId = null);
    Task<BibliotecaPermiso> AddAsync(BibliotecaPermiso entity);
    Task<BibliotecaPermiso?> UpdateAsync(BibliotecaPermiso entity);
    Task<bool> DeleteAsync(Guid id, string? username);
    Task<BibliotecaPermiso?> GetByNodeIdAndOrganizationalUnitIdAsync(Guid repositoryNodeId, Guid organizationalUnitId);
    Task<List<Guid>> GetReadableNodeIdsByOrganizationalUnitIdAsync(Guid organizationalUnitId);
}