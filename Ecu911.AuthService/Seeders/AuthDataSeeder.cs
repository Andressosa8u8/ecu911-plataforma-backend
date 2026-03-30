using Ecu911.AuthService.Data;
using Ecu911.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecu911.AuthService.Seeders;

public static class AuthDataSeeder
{
    public static async Task SeedAsync(AuthDbContext dbContext)
    {
        await SeedAdminAsync(dbContext);
    }

    private static async Task SeedAdminAsync(AuthDbContext dbContext)
    {
        const string username = "admin.ecu911";
        const string roleName = "ADMIN";
        const string moduleCode = "REPOSITORIO_DIGITAL";
        const string centroSigla = "UIO";

        var existingUser = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Username == username);

        if (existingUser != null)
            return;

        var adminRole = await dbContext.Roles
            .FirstOrDefaultAsync(x => x.Name == roleName);

        if (adminRole == null)
            throw new Exception("No existe el rol ADMIN. Debes ejecutar primero el seed de roles.");

        var repositorioModule = await dbContext.SystemModules
            .FirstOrDefaultAsync(x => x.Code == moduleCode);

        if (repositorioModule == null)
            throw new Exception("No existe el módulo REPOSITORIO_DIGITAL. Debes ejecutar primero el seed de módulos.");

        var provincia = await dbContext.Provinces
            .FirstOrDefaultAsync(x => x.Id == 17);

        if (provincia == null)
            throw new Exception("No existe la provincia con id 17.");

        var canton = await dbContext.Cantons
            .FirstOrDefaultAsync(x => x.Id == 177);

        if (canton == null)
            throw new Exception("No existe el cantón con id 177.");

        var centroZonal = await dbContext.CentrosZonales
            .FirstOrDefaultAsync(x => x.Sigla == centroSigla);

        if (centroZonal == null)
            throw new Exception("No existe el centro zonal con sigla UIO.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin.ecu911",
            Nombres = "Administrador",
            Apellidos = "Plataforma ECU911",
            Cedula = "0102030405",
            Email = "admin.ecu911@ecu911.gob.ec",
            Telefono = "0999999999",
            Cargo = "Administrador de Plataforma",
            ProvinciaId = provincia.Id,
            CantonId = canton.Id,
            CentroZonalId = centroZonal.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LastLoginAt = null,
            OrganizationalUnitId = null,

            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123")
        };

        dbContext.Users.Add(user);

        dbContext.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = adminRole.Id
        });

        dbContext.UserSystemRoles.Add(new UserSystemRole
        {
            UserId = user.Id,
            RoleId = adminRole.Id,
            SystemModuleId = repositorioModule.Id
        });

        await dbContext.SaveChangesAsync();
    }
}