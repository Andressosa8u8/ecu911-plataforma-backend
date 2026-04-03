namespace Ecu911.AuthService.DTOs;

public class RemoveUserRoleDto
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}