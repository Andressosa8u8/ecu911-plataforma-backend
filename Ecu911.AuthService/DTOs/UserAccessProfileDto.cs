namespace Ecu911.AuthService.DTOs;

public class UserAccessProfileDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = default!;
    public string Nombres { get; set; } = default!;
    public string Apellidos { get; set; } = default!;
    public string Cedula { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Telefono { get; set; }
    public string? Cargo { get; set; }

    public int? ProvinciaId { get; set; }
    public string? ProvinciaNombre { get; set; }

    public int? CantonId { get; set; }
    public string? CantonNombre { get; set; }

    public Guid? CentroZonalId { get; set; }
    public string? CentroZonalNombre { get; set; }

    public bool IsActive { get; set; }

    public List<UserGlobalRoleItemDto> GlobalRoles { get; set; } = new();
    public List<UserSystemRoleItemDto> SystemRoles { get; set; } = new();
    public List<UserSystemScopeItemDto> SystemScopes { get; set; } = new();
}