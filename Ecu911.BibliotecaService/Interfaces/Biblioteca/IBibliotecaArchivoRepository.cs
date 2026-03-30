using Ecu911.BibliotecaService.Models;

namespace Ecu911.BibliotecaService.Interfaces;

public interface IBibliotecaArchivoRepository
{
    Task<BibliotecaArchivo?> GetByBibliotecaDocumentoIdAsync(Guid documentItemId);
    Task<BibliotecaArchivo?> GetAnyByBibliotecaDocumentoIdAsync(Guid documentItemId);
    Task<BibliotecaArchivo> AddAsync(BibliotecaArchivo file);
    Task<BibliotecaArchivo> UpdateAsync(BibliotecaArchivo file);
}