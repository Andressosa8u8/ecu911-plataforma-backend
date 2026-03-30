using Ecu911.BibliotecaService.Data;
using Ecu911.BibliotecaService.Interfaces;
using Ecu911.BibliotecaService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.BibliotecaService.Repositories;

public class BibliotecaPermisoRepository : IBibliotecaPermisoRepository
{
    private readonly AppDbContext _context;

    public BibliotecaPermisoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BibliotecaPermiso>> GetAllAsync()
    {
        return await _context.BibliotecaPermisos
            .Include(x => x.BibliotecaColeccion)
            .Include(x => x.OrganizationalUnit)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.BibliotecaColeccion!.Name)
            .ThenBy(x => x.OrganizationalUnit!.Name)
            .ToListAsync();
    }

    public async Task<List<BibliotecaPermiso>> GetByNodeIdAsync(Guid repositoryNodeId)
    {
        return await _context.BibliotecaPermisos
            .Include(x => x.BibliotecaColeccion)
            .Include(x => x.OrganizationalUnit)
            .Where(x => !x.IsDeleted && x.BibliotecaColeccionId == repositoryNodeId)
            .OrderBy(x => x.OrganizationalUnit!.Name)
            .ToListAsync();
    }

    public async Task<List<BibliotecaPermiso>> GetByOrganizationalUnitIdAsync(Guid organizationalUnitId)
    {
        return await _context.BibliotecaPermisos
            .Include(x => x.BibliotecaColeccion)
            .Include(x => x.OrganizationalUnit)
            .Where(x => !x.IsDeleted && x.OrganizationalUnitId == organizationalUnitId)
            .OrderBy(x => x.BibliotecaColeccion!.Name)
            .ToListAsync();
    }

    public async Task<BibliotecaPermiso?> GetByIdAsync(Guid id)
    {
        return await _context.BibliotecaPermisos
            .Include(x => x.BibliotecaColeccion)
            .Include(x => x.OrganizationalUnit)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<bool> ExistsAsync(Guid repositoryNodeId, Guid organizationalUnitId, Guid? excludeId = null)
    {
        return await _context.BibliotecaPermisos.AnyAsync(x =>
            !x.IsDeleted &&
            x.BibliotecaColeccionId == repositoryNodeId &&
            x.OrganizationalUnitId == organizationalUnitId &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
    }

    public async Task<BibliotecaPermiso> AddAsync(BibliotecaPermiso entity)
    {
        _context.BibliotecaPermisos.Add(entity);
        await _context.SaveChangesAsync();
        return await GetByIdAsync(entity.Id) ?? entity;
    }

    public async Task<BibliotecaPermiso?> UpdateAsync(BibliotecaPermiso entity)
    {
        _context.BibliotecaPermisos.Update(entity);
        await _context.SaveChangesAsync();
        return await GetByIdAsync(entity.Id);
    }

    public async Task<bool> DeleteAsync(Guid id, string? username)
    {
        var entity = await _context.BibliotecaPermisos
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

    public async Task<BibliotecaPermiso?> GetByNodeIdAndOrganizationalUnitIdAsync(Guid repositoryNodeId, Guid organizationalUnitId)
    {
        return await _context.BibliotecaPermisos
            .Include(x => x.BibliotecaColeccion)
            .Include(x => x.OrganizationalUnit)
            .FirstOrDefaultAsync(x =>
                !x.IsDeleted &&
                x.BibliotecaColeccionId == repositoryNodeId &&
                x.OrganizationalUnitId == organizationalUnitId);
    }

    public async Task<List<Guid>> GetReadableNodeIdsByOrganizationalUnitIdAsync(Guid organizationalUnitId)
    {
        return await _context.BibliotecaPermisos
            .Where(x =>
                !x.IsDeleted &&
                x.OrganizationalUnitId == organizationalUnitId &&
                (x.CanView || x.CanManage))
            .Select(x => x.BibliotecaColeccionId)
            .Distinct()
            .ToListAsync();
    }
}