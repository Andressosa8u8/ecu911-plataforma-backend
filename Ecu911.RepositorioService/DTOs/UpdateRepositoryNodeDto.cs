namespace Ecu911.RepositorioService.DTOs;

public class UpdateRepositoryNodeDto
{
    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public Guid? OrganizationalUnitId { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public string Module { get; set; } = "REPOSITORIO";
}