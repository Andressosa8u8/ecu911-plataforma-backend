namespace Ecu911.AuthService.DTOs;

public class UserGlobalRoleItemDto
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = default!;
    public string RoleDescription { get; set; } = string.Empty;
    public string RoleType { get; set; } = default!;
    public bool IsActive { get; set; }
}