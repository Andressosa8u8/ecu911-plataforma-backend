using Ecu911.BibliotecaService.Data;
using Ecu911.BibliotecaService.Interfaces;
using Ecu911.BibliotecaService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.BibliotecaService.Repositories;

public class BibliotecaColeccionRepository : IBibliotecaColeccionRepository
{
    private readonly AppDbContext _context;

    public BibliotecaColeccionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BibliotecaColeccion>> GetAllAsync()
    {
        return await _context.BibliotecaColeccions
            .Include(x => x.Parent)
            .Include(x => x.OrganizationalUnit)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<BibliotecaColeccion>> GetRootsAsync()
    {
        return await _context.BibliotecaColeccions
            .Include(x => x.Parent)
            .Include(x => x.OrganizationalUnit)
            .Where(x => !x.IsDeleted && x.ParentId == null)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<List<BibliotecaColeccion>> GetChildrenAsync(Guid parentId)
    {
        return await _context.BibliotecaColeccions
            .Include(x => x.Parent)
            .Include(x => x.OrganizationalUnit)
            .Where(x => !x.IsDeleted && x.ParentId == parentId)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<BibliotecaColeccion?> GetByIdAsync(Guid id)
    {
        return await _context.BibliotecaColeccions
            .Include(x => x.Parent)
            .Include(x => x.OrganizationalUnit)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.BibliotecaColeccions
            .AnyAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<bool> ExistsActiveByNameAsync(string name, Guid? parentId, Guid? excludeId = null)
    {
        return await _context.BibliotecaColeccions.AnyAsync(x =>
            !x.IsDeleted &&
            x.Name.ToLower() == name.ToLower() &&
            x.ParentId == parentId &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
    }

    public async Task<bool> HasActiveChildrenAsync(Guid id)
    {
        return await _context.BibliotecaColeccions
            .AnyAsync(x => !x.IsDeleted && x.ParentId == id);
    }

    public async Task<BibliotecaColeccion> AddAsync(BibliotecaColeccion entity)
    {
        _context.BibliotecaColeccions.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<BibliotecaColeccion?> UpdateAsync(BibliotecaColeccion entity)
    {
        _context.BibliotecaColeccions.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id, string? username)
    {
        var entity = await _context.BibliotecaColeccions
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (entity == null)
        {
            return false;
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = username;

        await _context.SaveChangesAsync();
        return true;
    }
}