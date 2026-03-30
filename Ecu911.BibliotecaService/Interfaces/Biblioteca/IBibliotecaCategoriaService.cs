using Ecu911.BibliotecaService.DTOs;

namespace Ecu911.BibliotecaService.Interfaces;

public interface IBibliotecaCategoriaService
{
    Task<List<BibliotecaCategoriaDto>> GetAllAsync();
    Task<BibliotecaCategoriaDto?> GetByIdAsync(Guid id);
    Task<BibliotecaCategoriaDto> CreateAsync(CreateBibliotecaCategoriaDto input, string? username);
    Task<BibliotecaCategoriaDto?> UpdateAsync(Guid id, UpdateBibliotecaCategoriaDto input, string? username);
    Task<bool> DeleteAsync(Guid id, string? username);
    Task<BibliotecaCategoriaDto?> ActivateAsync(Guid id, string? username);
}