namespace Ecu911.BibliotecaService.DTOs;

public class CreateOrganizationalUnitDto
{
    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
}