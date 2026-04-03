namespace Ecu911.AuthService.DTOs;

public class RemoveUserSystemRoleDto
{
    public Guid UserId { get; set; }
    public Guid SystemModuleId { get; set; }
    public Guid RoleId { get; set; }
}