using Ecu911.RepositorioService.Data;
using Ecu911.RepositorioService.Interfaces;
using Ecu911.RepositorioService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.RepositorioService.Repositories;

public class DownloadAuditRepository : IDownloadAuditRepository
{
    private readonly AppDbContext _context;

    public DownloadAuditRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DownloadAudit>> GetAllAsync()
    {
        return await _context.DownloadAudits
            .Include(x => x.DocumentItem)
            .Include(x => x.DocumentFile)
            .OrderByDescending(x => x.DownloadedAt)
            .ToListAsync();
    }

    public async Task<List<DownloadAudit>> GetByDocumentItemIdAsync(Guid documentItemId)
    {
        return await _context.DownloadAudits
            .Include(x => x.DocumentItem)
            .Include(x => x.DocumentFile)
            .Where(x => x.DocumentItemId == documentItemId)
            .OrderByDescending(x => x.DownloadedAt)
            .ToListAsync();
    }

    public async Task<List<DownloadAudit>> GetByDocumentFileIdAsync(Guid documentFileId)
    {
        return await _context.DownloadAudits
            .Include(x => x.DocumentItem)
            .Include(x => x.DocumentFile)
            .Where(x => x.DocumentFileId == documentFileId)
            .OrderByDescending(x => x.DownloadedAt)
            .ToListAsync();
    }

    public async Task<DownloadAudit> AddAsync(DownloadAudit entity)
    {
        _context.DownloadAudits.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}