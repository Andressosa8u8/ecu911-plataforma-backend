namespace Ecu911.BibliotecaService.Interfaces;

public interface IBibliotecaAccessService
{
    Task<bool> CanViewNodeAsync(Guid repositoryNodeId, bool isAdmin, Guid? organizationalUnitId);
    Task<bool> CanUploadToNodeAsync(Guid repositoryNodeId, bool isAdmin, Guid? organizationalUnitId);
    Task<bool> CanDownloadFromNodeAsync(Guid repositoryNodeId, bool isAdmin, Guid? organizationalUnitId);
    Task<bool> CanDeleteFromNodeAsync(Guid repositoryNodeId, bool isAdmin, Guid? organizationalUnitId);
    Task<bool> CanManageNodeAsync(Guid repositoryNodeId, bool isAdmin, Guid? organizationalUnitId);
    Task<List<Guid>> GetReadableNodeIdsAsync(bool isAdmin, Guid? organizationalUnitId);
}