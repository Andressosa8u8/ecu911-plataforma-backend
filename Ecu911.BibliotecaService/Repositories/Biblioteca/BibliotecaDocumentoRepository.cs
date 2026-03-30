using Ecu911.BibliotecaService.Data;
using Ecu911.BibliotecaService.Interfaces;
using Ecu911.BibliotecaService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.BibliotecaService.Repositories;

public class BibliotecaDocumentoRepository : IBibliotecaDocumentoRepository
{
    private readonly AppDbContext _context;

    public BibliotecaDocumentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BibliotecaDocumento>> GetAllAsync()
    {
        return await _context.BibliotecaDocumentos
            .Include(x => x.BibliotecaCategoria)
            .Include(x => x.BibliotecaColeccion)
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<BibliotecaDocumento?> GetByIdAsync(Guid id)
    {
        return await _context.BibliotecaDocumentos
            .Include(x => x.BibliotecaCategoria)
            .Include(x => x.BibliotecaColeccion)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }

    public async Task<BibliotecaDocumento> AddAsync(BibliotecaDocumento item)
    {
        _context.BibliotecaDocumentos.Add(item);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(item.Id) ?? item;
    }

    public async Task<BibliotecaDocumento?> UpdateAsync(
        Guid id,
        string title,
        string description,
        Guid documentTypeId,
        Guid? repositoryNodeId,
        string? username)
    {
        var entity = await _context.BibliotecaDocumentos
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (entity == null)
        {
            return null;
        }

        entity.Title = title;
        entity.Description = description;
        entity.BibliotecaCategoriaId = documentTypeId;
        entity.BibliotecaColeccionId = repositoryNodeId;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = username;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id, string? username)
    {
        var entity = await _context.BibliotecaDocumentos
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

    public async Task<bool> ExistsActiveByBibliotecaCategoriaIdAsync(Guid documentTypeId)
    {
        return await _context.BibliotecaDocumentos
            .AnyAsync(x => x.BibliotecaCategoriaId == documentTypeId && !x.IsDeleted);
    }
}