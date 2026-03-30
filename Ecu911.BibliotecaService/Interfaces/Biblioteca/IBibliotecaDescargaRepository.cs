using Ecu911.BibliotecaService.Models;

namespace Ecu911.BibliotecaService.Interfaces;

public interface IBibliotecaDescargaRepository
{
    Task<List<BibliotecaDescarga>> GetAllAsync();
    Task<List<BibliotecaDescarga>> GetByBibliotecaDocumentoIdAsync(Guid documentItemId);
    Task<List<BibliotecaDescarga>> GetByBibliotecaArchivoIdAsync(Guid documentFileId);
    Task<BibliotecaDescarga> AddAsync(BibliotecaDescarga entity);
}