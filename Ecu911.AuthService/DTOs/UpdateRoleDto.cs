namespace Ecu911.AuthService.DTOs;

public class UpdateRoleDto
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public string RoleType { get; set; } = "FUNCIONAL";
}