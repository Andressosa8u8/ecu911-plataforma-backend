namespace Ecu911.BibliotecaService.DTOs;

public class BibliotecaDocumentoDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Guid BibliotecaCategoriaId { get; set; }
    public string BibliotecaCategoriaName { get; set; } = default!;

    public Guid? BibliotecaColeccionId { get; set; }
    public string? BibliotecaColeccionName { get; set; }
}