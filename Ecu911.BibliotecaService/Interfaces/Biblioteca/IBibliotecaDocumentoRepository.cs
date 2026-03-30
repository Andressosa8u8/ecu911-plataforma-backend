using Ecu911.BibliotecaService.Models;

namespace Ecu911.BibliotecaService.Interfaces;

public interface IBibliotecaDocumentoRepository
{
    Task<List<BibliotecaDocumento>> GetAllAsync();
    Task<BibliotecaDocumento?> GetByIdAsync(Guid id);
    Task<BibliotecaDocumento> AddAsync(BibliotecaDocumento item);
    Task<BibliotecaDocumento?> UpdateAsync(
        Guid id,
        string title,
        string description,
        Guid documentTypeId,
        Guid? repositoryNodeId,
        string? username);
    Task<bool> DeleteAsync(Guid id, string? username);
    Task<bool> ExistsActiveByBibliotecaCategoriaIdAsync(Guid documentTypeId);
}