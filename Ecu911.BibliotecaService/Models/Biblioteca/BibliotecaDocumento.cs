using System.ComponentModel.DataAnnotations;

namespace Ecu911.BibliotecaService.Models;

public class BibliotecaDocumento
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = default!;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public Guid BibliotecaCategoriaId { get; set; }
    public BibliotecaCategoria? BibliotecaCategoria { get; set; }

    public Guid? BibliotecaColeccionId { get; set; }
    public BibliotecaColeccion? BibliotecaColeccion { get; set; }

    public BibliotecaArchivo? BibliotecaArchivo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public ICollection<BibliotecaDescarga> BibliotecaDescargas { get; set; } = new List<BibliotecaDescarga>();
}