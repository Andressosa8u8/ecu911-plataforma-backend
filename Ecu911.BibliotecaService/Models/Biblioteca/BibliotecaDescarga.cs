namespace Ecu911.BibliotecaService.Models;

public class BibliotecaDescarga
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid BibliotecaDocumentoId { get; set; }
    public BibliotecaDocumento? BibliotecaDocumento { get; set; }

    public Guid BibliotecaArchivoId { get; set; }
    public BibliotecaArchivo? BibliotecaArchivo { get; set; }

    public DateTime DownloadedAt { get; set; } = DateTime.UtcNow;
    public string? DownloadedBy { get; set; }
}