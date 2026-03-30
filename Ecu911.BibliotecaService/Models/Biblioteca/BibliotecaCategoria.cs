namespace Ecu911.BibliotecaService.Models;

public class BibliotecaCategoria
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<BibliotecaDocumento> BibliotecaDocumentos { get; set; } = new List<BibliotecaDocumento>();
}