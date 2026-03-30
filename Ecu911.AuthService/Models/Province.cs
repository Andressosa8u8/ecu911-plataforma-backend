namespace Ecu911.AuthService.Models;

public class Province
{
    public int Id { get; set; }
    public string Nombre { get; set; } = default!;
    public string? Codigo { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Canton> Cantones { get; set; } = new List<Canton>();
    public ICollection<CentroZonal> CentrosZonales { get; set; } = new List<CentroZonal>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
