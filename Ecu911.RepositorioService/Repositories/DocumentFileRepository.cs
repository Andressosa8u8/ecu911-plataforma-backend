using Ecu911.RepositorioService.Data;
using Ecu911.RepositorioService.Interfaces;
using Ecu911.RepositorioService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.RepositorioService.Repositories
{
    public class DocumentFileRepository : IDocumentFileRepository
    {
        private readonly AppDbContext _context;

        public DocumentFileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DocumentFile?> GetByDocumentItemIdAsync(Guid documentItemId)
        {
            return await _context.DocumentFiles
                .FirstOrDefaultAsync(x => x.DocumentItemId == documentItemId && !x.IsDeleted);
        }

        public async Task<DocumentFile?> GetAnyByDocumentItemIdAsync(Guid documentItemId)
        {
            return await _context.DocumentFiles
                .FirstOrDefaultAsync(x => x.DocumentItemId == documentItemId);
        }

        public async Task<DocumentFile> AddAsync(DocumentFile file)
        {
            _context.DocumentFiles.Add(file);
            await _context.SaveChangesAsync();
            return file;
        }

        public async Task<DocumentFile> UpdateAsync(DocumentFile file)
        {
            _context.DocumentFiles.Update(file);
            await _context.SaveChangesAsync();
            return file;
        }
    }
}