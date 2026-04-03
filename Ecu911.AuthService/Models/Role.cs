using Ecu911.AuthService.Models;

public class Role
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public string RoleType { get; set; } = "FUNCIONAL";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserSystemRole> UserSystemRoles { get; set; } = new List<UserSystemRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}