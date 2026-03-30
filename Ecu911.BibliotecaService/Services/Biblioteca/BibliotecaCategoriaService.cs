using Ecu911.BibliotecaService.DTOs;
using Ecu911.BibliotecaService.Interfaces;
using Ecu911.BibliotecaService.Models;

namespace Ecu911.BibliotecaService.Services;

public class BibliotecaCategoriaService : IBibliotecaCategoriaService
{
    private readonly IBibliotecaCategoriaRepository _repository;
    private readonly AuditService _auditService;

    public BibliotecaCategoriaService(
        IBibliotecaCategoriaRepository repository,
        AuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
    }

    public async Task<List<BibliotecaCategoriaDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();

        return items.Select(x => new BibliotecaCategoriaDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt
        }).ToList();
    }

    public async Task<BibliotecaCategoriaDto?> GetByIdAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return null;

        return new BibliotecaCategoriaDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            IsActive = item.IsActive,
            CreatedAt = item.CreatedAt
        };
    }

    public async Task<BibliotecaCategoriaDto> CreateAsync(CreateBibliotecaCategoriaDto input, string? username)
    {
        if (await _repository.NameExistsAsync(input.Name))
            throw new Exception("Ya existe un tipo documental activo con ese nombre.");

        var entity = new BibliotecaCategoria
        {
            Id = Guid.NewGuid(),
            Name = input.Name,
            Description = input.Description ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var created = await _repository.AddAsync(entity);

        _auditService.LogAction("Create", username ?? "Unknown", $"Created BibliotecaCategoria: {created.Name}");

        return new BibliotecaCategoriaDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt
        };
    }

    public async Task<BibliotecaCategoriaDto?> UpdateAsync(Guid id, UpdateBibliotecaCategoriaDto input, string? username)
    {
        var exists = await _repository.ExistsAsync(id);
        if (!exists) return null;

        if (await _repository.NameExistsForOtherAsync(id, input.Name))
            throw new Exception("Ya existe otro tipo documental activo con ese nombre.");

        var updated = await _repository.UpdateAsync(id, input.Name, input.Description ?? string.Empty);
        if (updated == null) return null;

        _auditService.LogAction("Update", username ?? "Unknown", $"Updated BibliotecaCategoria: {updated.Name}");

        return new BibliotecaCategoriaDto
        {
            Id = updated.Id,
            Name = updated.Name,
            Description = updated.Description,
            IsActive = updated.IsActive,
            CreatedAt = updated.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id, string? username)
    {
        var deleted = await _repository.DeactivateAsync(id);

        if (deleted)
            _auditService.LogAction("Delete", username ?? "Unknown", $"Deactivated BibliotecaCategoria with ID: {id}");

        return deleted;
    }

    public async Task<BibliotecaCategoriaDto?> ActivateAsync(Guid id, string? username)
    {
        var exists = await _repository.ExistsAsync(id);
        if (!exists) return null;

        var activated = await _repository.ActivateAsync(id);
        if (!activated)
        {
            var current = await _repository.GetByIdAsync(id);
            if (current == null) return null;

            return new BibliotecaCategoriaDto
            {
                Id = current.Id,
                Name = current.Name,
                Description = current.Description,
                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt
            };
        }

        var item = await _repository.GetByIdAsync(id);
        if (item == null) return null;

        _auditService.LogAction("Activate", username ?? "Unknown", $"Activated BibliotecaCategoria with ID: {id}");

        return new BibliotecaCategoriaDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            IsActive = item.IsActive,
            CreatedAt = item.CreatedAt
        };
    }
}