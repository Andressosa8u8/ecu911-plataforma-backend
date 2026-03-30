namespace Ecu911.RepositorioService.DTOs;

public class UpdateDocumentItemDto
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public Guid DocumentTypeId { get; set; }
    public Guid RepositoryNodeId { get; set; }
}