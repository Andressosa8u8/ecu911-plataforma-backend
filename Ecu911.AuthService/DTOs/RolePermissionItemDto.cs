namespace Ecu911.AuthService.DTOs;

public class RolePermissionItemDto
{
    public Guid PermissionId { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}