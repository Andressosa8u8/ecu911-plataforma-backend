using Ecu911.CatalogService.Models;

namespace Ecu911.CatalogService.Interfaces;

public interface IDocumentTypeRepository
{
    Task<List<DocumentType>> GetAllAsync();
    Task<DocumentType?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> NameExistsAsync(string name);
    Task<bool> NameExistsForOtherAsync(Guid excludeId, string name);
    Task<DocumentType> AddAsync(DocumentType entity);
    Task<DocumentType?> UpdateAsync(Guid id, string name, string description);
    Task<bool> DeactivateAsync(Guid id);
    Task<bool> ActivateAsync(Guid id);
}