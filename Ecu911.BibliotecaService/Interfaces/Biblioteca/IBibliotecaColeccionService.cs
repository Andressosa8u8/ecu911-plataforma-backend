using Ecu911.BibliotecaService.DTOs;

namespace Ecu911.BibliotecaService.Interfaces;

public interface IBibliotecaColeccionService
{
    Task<List<BibliotecaColeccionDto>> GetAllAsync();
    Task<List<BibliotecaColeccionDto>> GetRootsAsync();
    Task<List<BibliotecaColeccionDto>> GetChildrenAsync(Guid parentId);
    Task<BibliotecaColeccionDto?> GetByIdAsync(Guid id);
    Task<BibliotecaColeccionDto> CreateAsync(CreateBibliotecaColeccionDto input, string? username);
    Task<BibliotecaColeccionDto?> UpdateAsync(Guid id, UpdateBibliotecaColeccionDto input, string? username);
    Task<bool> DeleteAsync(Guid id, string? username);
}