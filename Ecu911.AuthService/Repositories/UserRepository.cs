using Ecu911.AuthService.Data;
using Ecu911.AuthService.Interfaces;
using Ecu911.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.AuthService.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    private IQueryable<User> Query() => _context.Users
        .Include(x => x.Provincia)
        .Include(x => x.Canton)
        .Include(x => x.CentroZonal)
        .Include(x => x.UserRoles).ThenInclude(x => x.Role)
        .Include(x => x.UserSystemRoles).ThenInclude(x => x.Role)
        .Include(x => x.UserSystemRoles).ThenInclude(x => x.SystemModule);

    public Task<User?> GetByUsernameAsync(string username) =>
        Query().FirstOrDefaultAsync(x => x.Username == username);

    public Task<User?> GetByEmailAsync(string email) =>
        Query().FirstOrDefaultAsync(x => x.Email == email);

    public Task<User?> GetByCedulaAsync(string cedula) =>
        Query().FirstOrDefaultAsync(x => x.Cedula == cedula);

    public Task<User?> GetByIdAsync(Guid id) =>
        Query().FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<User>> GetAllAsync() =>
        Query().ToListAsync();

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}