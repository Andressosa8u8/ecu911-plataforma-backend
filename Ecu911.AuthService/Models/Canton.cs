namespace Ecu911.AuthService.Models;

public class Canton
{
    public int Id { get; set; }
    public int ProvinciaId { get; set; }
    public Province Provincia { get; set; } = default!;
    public string Nombre { get; set; } = default!;
    public string? Codigo { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<User> Users { get; set; } = new List<User>();
}
