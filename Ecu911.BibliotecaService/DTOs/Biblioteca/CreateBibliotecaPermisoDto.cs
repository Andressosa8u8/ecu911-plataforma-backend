namespace Ecu911.BibliotecaService.DTOs;

public class CreateBibliotecaPermisoDto
{
    public Guid BibliotecaColeccionId { get; set; }
    public Guid OrganizationalUnitId { get; set; }
    public bool CanView { get; set; }
    public bool CanUpload { get; set; }
    public bool CanDownload { get; set; }
    public bool CanDelete { get; set; }
    public bool CanManage { get; set; }
}