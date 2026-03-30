namespace Ecu911.BibliotecaService.DTOs;

public class BibliotecaArchivoDownloadDto
{
    public Guid BibliotecaArchivoId { get; set; }
    public string AbsolutePath { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string FileName { get; set; } = default!;
}