using Ecu911.BibliotecaService.Data;
using Ecu911.BibliotecaService.Interfaces;
using Ecu911.BibliotecaService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.BibliotecaService.Repositories;

public class BibliotecaDescargaRepository : IBibliotecaDescargaRepository
{
    private readonly AppDbContext _context;

    public BibliotecaDescargaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BibliotecaDescarga>> GetAllAsync()
    {
        return await _context.BibliotecaDescargas
            .Include(x => x.BibliotecaDocumento)
            .Include(x => x.BibliotecaArchivo)
            .OrderByDescending(x => x.DownloadedAt)
            .ToListAsync();
    }

    public async Task<List<BibliotecaDescarga>> GetByBibliotecaDocumentoIdAsync(Guid documentItemId)
    {
        return await _context.BibliotecaDescargas
            .Include(x => x.BibliotecaDocumento)
            .Include(x => x.BibliotecaArchivo)
            .Where(x => x.BibliotecaDocumentoId == documentItemId)
            .OrderByDescending(x => x.DownloadedAt)
            .ToListAsync();
    }

    public async Task<List<BibliotecaDescarga>> GetByBibliotecaArchivoIdAsync(Guid documentFileId)
    {
        return await _context.BibliotecaDescargas
            .Include(x => x.BibliotecaDocumento)
            .Include(x => x.BibliotecaArchivo)
            .Where(x => x.BibliotecaArchivoId == documentFileId)
            .OrderByDescending(x => x.DownloadedAt)
            .ToListAsync();
    }

    public async Task<BibliotecaDescarga> AddAsync(BibliotecaDescarga entity)
    {
        _context.BibliotecaDescargas.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}