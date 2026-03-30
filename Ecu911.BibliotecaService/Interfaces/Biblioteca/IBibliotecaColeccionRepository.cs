using Ecu911.BibliotecaService.Models;

namespace Ecu911.BibliotecaService.Interfaces;

public interface IBibliotecaColeccionRepository
{
    Task<List<BibliotecaColeccion>> GetAllAsync();
    Task<List<BibliotecaColeccion>> GetRootsAsync();
    Task<List<BibliotecaColeccion>> GetChildrenAsync(Guid parentId);
    Task<BibliotecaColeccion?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsActiveByNameAsync(string name, Guid? parentId, Guid? excludeId = null);
    Task<bool> HasActiveChildrenAsync(Guid id);
    Task<BibliotecaColeccion> AddAsync(BibliotecaColeccion entity);
    Task<BibliotecaColeccion?> UpdateAsync(BibliotecaColeccion entity);
    Task<bool> DeleteAsync(Guid id, string? username);
}