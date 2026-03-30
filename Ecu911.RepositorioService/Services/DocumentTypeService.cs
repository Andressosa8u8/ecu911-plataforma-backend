using Ecu911.RepositorioService.DTOs;
using Ecu911.RepositorioService.Interfaces;
using Ecu911.RepositorioService.Models;

namespace Ecu911.RepositorioService.Services;

public class DocumentTypeService : IDocumentTypeService
{
    private readonly IDocumentTypeRepository _repository;
    private readonly AuditService _auditService;

    public DocumentTypeService(
        IDocumentTypeRepository repository,
        AuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
    }

    public async Task<List<DocumentTypeDto>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();

        return items.Select(x => new DocumentTypeDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt
        }).ToList();
    }

    public async Task<DocumentTypeDto?> GetByIdAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return null;

        return new DocumentTypeDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            IsActive = item.IsActive,
            CreatedAt = item.CreatedAt
        };
    }

    public async Task<DocumentTypeDto> CreateAsync(CreateDocumentTypeDto input, string? username)
    {
        if (await _repository.NameExistsAsync(input.Name))
            throw new Exception("Ya existe un tipo documental activo con ese nombre.");

        var entity = new DocumentType
        {
            Id = Guid.NewGuid(),
            Name = input.Name,
            Description = input.Description ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var created = await _repository.AddAsync(entity);

        _auditService.LogAction("Create", username ?? "Unknown", $"Created DocumentType: {created.Name}");

        return new DocumentTypeDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt
        };
    }

    public async Task<DocumentTypeDto?> UpdateAsync(Guid id, UpdateDocumentTypeDto input, string? username)
    {
        var exists = await _repository.ExistsAsync(id);
        if (!exists) return null;

        if (await _repository.NameExistsForOtherAsync(id, input.Name))
            throw new Exception("Ya existe otro tipo documental activo con ese nombre.");

        var updated = await _repository.UpdateAsync(id, input.Name, input.Description ?? string.Empty);
        if (updated == null) return null;

        _auditService.LogAction("Update", username ?? "Unknown", $"Updated DocumentType: {updated.Name}");

        return new DocumentTypeDto
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
            _auditService.LogAction("Delete", username ?? "Unknown", $"Deactivated DocumentType with ID: {id}");

        return deleted;
    }

    public async Task<DocumentTypeDto?> ActivateAsync(Guid id, string? username)
    {
        var exists = await _repository.ExistsAsync(id);
        if (!exists) return null;

        var activated = await _repository.ActivateAsync(id);
        if (!activated)
        {
            var current = await _repository.GetByIdAsync(id);
            if (current == null) return null;

            return new DocumentTypeDto
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

        _auditService.LogAction("Activate", username ?? "Unknown", $"Activated DocumentType with ID: {id}");

        return new DocumentTypeDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            IsActive = item.IsActive,
            CreatedAt = item.CreatedAt
        };
    }
}