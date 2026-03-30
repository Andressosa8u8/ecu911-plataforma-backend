namespace Ecu911.AuthService.DTOs;

public class CantonDto
{
    public int Id { get; set; }
    public int ProvinciaId { get; set; }
    public string Nombre { get; set; } = default!;
}
