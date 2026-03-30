using Ecu911.BibliotecaService.Models;

namespace Ecu911.BibliotecaService.Interfaces;

public interface IBibliotecaCategoriaRepository
{
    Task<List<BibliotecaCategoria>> GetAllAsync();
    Task<BibliotecaCategoria?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> NameExistsAsync(string name);
    Task<bool> NameExistsForOtherAsync(Guid excludeId, string name);
    Task<BibliotecaCategoria> AddAsync(BibliotecaCategoria entity);
    Task<BibliotecaCategoria?> UpdateAsync(Guid id, string name, string description);
    Task<bool> DeactivateAsync(Guid id);
    Task<bool> ActivateAsync(Guid id);
}