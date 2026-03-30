namespace Ecu911.AuthService.DTOs;

public class CreateUserDto
{
    public string Username { get; set; } = default!;
    public string Nombres { get; set; } = default!;
    public string Apellidos { get; set; } = default!;
    public string Cedula { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Telefono { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Cargo { get; set; } = default!;
    public int ProvinciaId { get; set; }
    public int CantonId { get; set; }
    public Guid CentroZonalId { get; set; }
    public Guid? OrganizationalUnitId { get; set; }
}
