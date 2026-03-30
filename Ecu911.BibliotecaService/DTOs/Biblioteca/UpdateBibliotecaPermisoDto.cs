namespace Ecu911.BibliotecaService.DTOs;

public class UpdateBibliotecaPermisoDto
{
    public bool CanView { get; set; }
    public bool CanUpload { get; set; }
    public bool CanDownload { get; set; }
    public bool CanDelete { get; set; }
    public bool CanManage { get; set; }
}