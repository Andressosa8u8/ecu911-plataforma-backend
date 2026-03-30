namespace Ecu911.BibliotecaService.Models;

public class BibliotecaArchivo
{
    public Guid Id { get; set; }

    public Guid BibliotecaDocumentoId { get; set; }
    public BibliotecaDocumento BibliotecaDocumento { get; set; } = default!;

    public string OriginalFileName { get; set; } = default!;
    public string StoredFileName { get; set; } = default!;
    public string RelativePath { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string Extension { get; set; } = default!;

    public long SizeInBytes { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public string? UploadedBy { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public ICollection<BibliotecaDescarga> BibliotecaDescargas { get; set; } = new List<BibliotecaDescarga>();
}