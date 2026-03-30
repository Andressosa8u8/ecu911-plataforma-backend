namespace Ecu911.AuthService.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Nombres { get; set; } = default!;
    public string Apellidos { get; set; } = default!;
    public string Cedula { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Telefono { get; set; } = default!;
    public string Cargo { get; set; } = default!;
    public int ProvinciaId { get; set; }
    public string Provincia { get; set; } = default!;
    public int CantonId { get; set; }
    public string Canton { get; set; } = default!;
    public Guid CentroZonalId { get; set; }
    public string CentroZonal { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public Guid? OrganizationalUnitId { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Systems { get; set; } = new();
    public string? CurrentSystem { get; set; }
}
