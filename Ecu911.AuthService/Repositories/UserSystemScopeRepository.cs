using Ecu911.AuthService.Data;
using Ecu911.AuthService.Interfaces;
using Ecu911.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.AuthService.Repositories;

public class UserSystemScopeRepository : IUserSystemScopeRepository
{
    private readonly AuthDbContext _context;

    public UserSystemScopeRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<UserSystemScope?> GetByUserAndSystemAsync(Guid userId, Guid systemModuleId)
    {
        return await _context.UserSystemScopes
            .FirstOrDefaultAsync(x => x.UserId == userId && x.SystemModuleId == systemModuleId);
    }

    public async Task<UserSystemScope?> GetExactAsync(Guid userId, Guid systemModuleId, string scopeLevel, string? centerCode, string? jurisdictionCode)
    {
        return await _context.UserSystemScopes.FirstOrDefaultAsync(x =>
            x.UserId == userId &&
            x.SystemModuleId == systemModuleId &&
            x.ScopeLevel == scopeLevel &&
            x.CenterCode == centerCode &&
            x.JurisdictionCode == jurisdictionCode);
    }

    public async Task<List<UserSystemScope>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserSystemScopes
            .Include(x => x.SystemModule)
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.SystemModule.Name)
            .ThenBy(x => x.ScopeLevel)
            .ToListAsync();
    }

    public async Task AddAsync(UserSystemScope entity)
    {
        _context.UserSystemScopes.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserSystemScope entity)
    {
        _context.UserSystemScopes.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(UserSystemScope entity)
    {
        _context.UserSystemScopes.Remove(entity);
        await _context.SaveChangesAsync();
    }
}