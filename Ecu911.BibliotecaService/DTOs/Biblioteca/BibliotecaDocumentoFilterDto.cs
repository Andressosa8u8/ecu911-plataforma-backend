namespace Ecu911.BibliotecaService.DTOs;

public class BibliotecaDocumentoFilterDto
{
    public string? Title { get; set; }
    public Guid? BibliotecaCategoriaId { get; set; }
    public Guid? BibliotecaColeccionId { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}