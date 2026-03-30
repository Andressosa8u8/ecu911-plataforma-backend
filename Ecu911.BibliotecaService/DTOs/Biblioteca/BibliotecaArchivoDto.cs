namespace Ecu911.BibliotecaService.DTOs;

public class BibliotecaArchivoDto
{
    public Guid Id { get; set; }
    public Guid BibliotecaDocumentoId { get; set; }
    public string OriginalFileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public long SizeInBytes { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? UploadedBy { get; set; }
}