using Ecu911.BibliotecaService.Data;
using Ecu911.BibliotecaService.Interfaces;
using Ecu911.BibliotecaService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.BibliotecaService.Repositories
{
    public class BibliotecaArchivoRepository : IBibliotecaArchivoRepository
    {
        private readonly AppDbContext _context;

        public BibliotecaArchivoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BibliotecaArchivo?> GetByBibliotecaDocumentoIdAsync(Guid documentItemId)
        {
            return await _context.BibliotecaArchivos
                .FirstOrDefaultAsync(x => x.BibliotecaDocumentoId == documentItemId && !x.IsDeleted);
        }

        public async Task<BibliotecaArchivo?> GetAnyByBibliotecaDocumentoIdAsync(Guid documentItemId)
        {
            return await _context.BibliotecaArchivos
                .FirstOrDefaultAsync(x => x.BibliotecaDocumentoId == documentItemId);
        }

        public async Task<BibliotecaArchivo> AddAsync(BibliotecaArchivo file)
        {
            _context.BibliotecaArchivos.Add(file);
            await _context.SaveChangesAsync();
            return file;
        }

        public async Task<BibliotecaArchivo> UpdateAsync(BibliotecaArchivo file)
        {
            _context.BibliotecaArchivos.Update(file);
            await _context.SaveChangesAsync();
            return file;
        }
    }
}