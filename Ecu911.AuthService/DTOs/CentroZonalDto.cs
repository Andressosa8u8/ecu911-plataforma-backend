namespace Ecu911.AuthService.DTOs;

public class CentroZonalDto
{
    public Guid Id { get; set; }
    public int ProvinciaId { get; set; }
    public string Nombre { get; set; } = default!;
    public string Sigla { get; set; } = default!;
    public int Grupo { get; set; }
    public Guid? ParentId { get; set; }
}
