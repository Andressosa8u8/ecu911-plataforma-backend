namespace Ecu911.BibliotecaService.DTOs;

public class CreateBibliotecaDocumentoDto
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public Guid BibliotecaCategoriaId { get; set; }
    public Guid BibliotecaColeccionId { get; set; }
}