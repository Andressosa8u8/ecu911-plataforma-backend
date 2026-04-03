namespace Ecu911.AuthService.DTOs;

public class UserSystemRoleItemDto
{
    public Guid SystemModuleId { get; set; }
    public string SystemCode { get; set; } = default!;
    public string SystemName { get; set; } = default!;
    public bool SystemIsActive { get; set; }

    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = default!;
    public string RoleDescription { get; set; } = string.Empty;
    public string RoleType { get; set; } = default!;
    public bool RoleIsActive { get; set; }
}