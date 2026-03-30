using Ecu911.RepositorioService.DTOs;

namespace Ecu911.RepositorioService.Interfaces;

public interface IDownloadAuditService
{
    Task<List<DownloadAuditDto>> GetAllAsync();
    Task<List<DownloadAuditDto>> GetByDocumentItemIdAsync(Guid documentItemId);
    Task<List<DownloadAuditDto>> GetByDocumentFileIdAsync(Guid documentFileId);
    Task RegisterAsync(Guid documentItemId, Guid documentFileId, string? username);
}