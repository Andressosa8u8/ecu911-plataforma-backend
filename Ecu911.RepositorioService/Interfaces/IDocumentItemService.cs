using Ecu911.RepositorioService.DTOs;

namespace Ecu911.RepositorioService.Interfaces;

public interface IDocumentItemService
{
    Task<PagedResultDto<DocumentItemDto>> GetAllAsync(DocumentItemFilterDto filter, int pageIndex, int pageSize, bool isAdmin, Guid? organizationalUnitId);
    Task<DocumentItemDto?> GetByIdAsync(Guid id, bool isAdmin, Guid? organizationalUnitId);
    Task<DocumentItemDto> CreateAsync(CreateDocumentItemDto input, string? username, bool isAdmin, Guid? organizationalUnitId);
    Task<DocumentItemDto?> UpdateAsync(Guid id, UpdateDocumentItemDto input, string? username, bool isAdmin, Guid? organizationalUnitId);
    Task<bool> DeleteAsync(Guid id, string? username, bool isAdmin, Guid? organizationalUnitId);
}