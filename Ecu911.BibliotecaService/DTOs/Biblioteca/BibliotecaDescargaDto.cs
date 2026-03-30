namespace Ecu911.BibliotecaService.DTOs;

public class BibliotecaDescargaDto
{
    public Guid Id { get; set; }
    public Guid BibliotecaDocumentoId { get; set; }
    public string DocumentTitle { get; set; } = default!;
    public Guid BibliotecaArchivoId { get; set; }
    public string FileName { get; set; } = default!;
    public DateTime DownloadedAt { get; set; }
    public string? DownloadedBy { get; set; }
}