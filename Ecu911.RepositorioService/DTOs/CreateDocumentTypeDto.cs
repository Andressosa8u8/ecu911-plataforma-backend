namespace Ecu911.RepositorioService.DTOs;

public class CreateDocumentTypeDto
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
}