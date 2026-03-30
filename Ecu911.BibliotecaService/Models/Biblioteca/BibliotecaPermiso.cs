using System.ComponentModel.DataAnnotations;

namespace Ecu911.BibliotecaService.Models;

public class BibliotecaPermiso
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid BibliotecaColeccionId { get; set; }
    public BibliotecaColeccion? BibliotecaColeccion { get; set; }

    public Guid OrganizationalUnitId { get; set; }
    public OrganizationalUnit? OrganizationalUnit { get; set; }

    public bool CanView { get; set; } = false;
    public bool CanUpload { get; set; } = false;
    public bool CanDownload { get; set; } = false;
    public bool CanDelete { get; set; } = false;
    public bool CanManage { get; set; } = false;

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}