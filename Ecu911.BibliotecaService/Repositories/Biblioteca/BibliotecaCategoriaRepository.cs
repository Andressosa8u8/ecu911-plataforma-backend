using Ecu911.BibliotecaService.Data;
using Ecu911.BibliotecaService.Interfaces;
using Ecu911.BibliotecaService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.BibliotecaService.Repositories;

public class BibliotecaCategoriaRepository : IBibliotecaCategoriaRepository
{
    private readonly AppDbContext _context;

    public BibliotecaCategoriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BibliotecaCategoria>> GetAllAsync()
    {
        return await _context.BibliotecaCategorias
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<BibliotecaCategoria?> GetByIdAsync(Guid id)
    {
        return await _context.BibliotecaCategorias
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.BibliotecaCategorias.AnyAsync(x => x.Id == id);
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.BibliotecaCategorias
            .AnyAsync(x => x.IsActive && x.Name.ToLower() == name.ToLower());
    }

    public async Task<bool> NameExistsForOtherAsync(Guid excludeId, string name)
    {
        return await _context.BibliotecaCategorias
            .AnyAsync(x => x.IsActive && x.Id != excludeId && x.Name.ToLower() == name.ToLower());
    }

    public async Task<BibliotecaCategoria> AddAsync(BibliotecaCategoria entity)
    {
        _context.BibliotecaCategorias.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<BibliotecaCategoria?> UpdateAsync(Guid id, string name, string description)
    {
        var existing = await _context.BibliotecaCategorias
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
        var existing = await _context.BibliotecaCategorias
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

        if (existing == null)
            return false;

        existing.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActivateAsync(Guid id)
    {
        var existing = await _context.BibliotecaCategorias
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsActive);

        if (existing == null)
            return false;

        existing.IsActive = true;
        await _context.SaveChangesAsync();
        return true;
    }
}