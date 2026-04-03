using Ecu911.AuthService.Models;

namespace Ecu911.AuthService.Interfaces;

public interface IUserSystemScopeRepository
{
    Task<UserSystemScope?> GetByUserAndSystemAsync(Guid userId, Guid systemModuleId);
    Task<UserSystemScope?> GetExactAsync(Guid userId, Guid systemModuleId, string scopeLevel, string? centerCode, string? jurisdictionCode);
    Task<List<UserSystemScope>> GetByUserIdAsync(Guid userId);
    Task AddAsync(UserSystemScope entity);
    Task UpdateAsync(UserSystemScope entity);
    Task RemoveAsync(UserSystemScope entity);
}