using Ecu911.RepositorioService.Data;
using Ecu911.RepositorioService.Interfaces;
using Ecu911.RepositorioService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.RepositorioService.Repositories;

public class DocumentTypeRepository : IDocumentTypeRepository
{
    private readonly AppDbContext _context;

    public DocumentTypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DocumentType>> GetAllAsync()
    {
        return await _context.DocumentTypes
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<DocumentType?> GetByIdAsync(Guid id)
    {
        return await _context.DocumentTypes
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.DocumentTypes.AnyAsync(x => x.Id == id);
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.DocumentTypes
            .AnyAsync(x => x.IsActive && x.Name.ToLower() == name.ToLower());
    }

    public async Task<bool> NameExistsForOtherAsync(Guid excludeId, string name)
    {
        return await _context.DocumentTypes
            .AnyAsync(x => x.IsActive && x.Id != excludeId && x.Name.ToLower() == name.ToLower());
    }

    public async Task<DocumentType> AddAsync(DocumentType entity)
    {
        _context.DocumentTypes.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<DocumentType?> UpdateAsync(Guid id, string name, string description)
    {
        var existing = await _context.DocumentTypes
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existing == null)
            return null;

        existing.Name = name;
        existing.Description = description;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var existing = await _context.DocumentTypes
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

        if (existing == null)
            return false;

        existing.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActivateAsync(Guid id)
    {
        var existing = await _context.DocumentTypes
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsActive);

        if (existing == null)
            return false;

        existing.IsActive = true;
        await _context.SaveChangesAsync();
        return true;
    }
}