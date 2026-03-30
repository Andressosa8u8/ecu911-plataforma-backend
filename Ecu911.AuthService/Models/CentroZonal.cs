namespace Ecu911.AuthService.Models;

public class CentroZonal
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public CentroZonal? Parent { get; set; }
    public ICollection<CentroZonal> Children { get; set; } = new List<CentroZonal>();
    public int Grupo { get; set; }
    public int ProvinciaId { get; set; }
    public Province Provincia { get; set; } = default!;
    public string Nombre { get; set; } = default!;
    public string Sigla { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    public ICollection<User> Users { get; set; } = new List<User>();
}
