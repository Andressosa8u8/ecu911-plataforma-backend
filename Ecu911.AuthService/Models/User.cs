namespace Ecu911.AuthService.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Nombres { get; set; } = default!;
    public string Apellidos { get; set; } = default!;
    public string Cedula { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Telefono { get; set; } = default!;
    public string Cargo { get; set; } = default!;
    public int ProvinciaId { get; set; }
    public Province Provincia { get; set; } = default!;
    public int CantonId { get; set; }
    public Canton Canton { get; set; } = default!;
    public Guid CentroZonalId { get; set; }
    public CentroZonal CentroZonal { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public Guid? OrganizationalUnitId { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserSystemRole> UserSystemRoles { get; set; } = new List<UserSystemRole>();
    public ICollection<UserSystemScope> UserSystemScopes { get; set; } = new List<UserSystemScope>();
}
