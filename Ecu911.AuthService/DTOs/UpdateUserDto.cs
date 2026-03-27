namespace Ecu911.AuthService.DTOs;

public class UpdateUserDto
{
    public string Username { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool IsActive { get; set; }
    public Guid? OrganizationalUnitId { get; set; }
}