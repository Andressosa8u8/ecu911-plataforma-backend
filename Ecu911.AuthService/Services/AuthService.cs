using Ecu911.AuthService.Data;
using Ecu911.AuthService.DTOs;
using Ecu911.AuthService.Helpers;
using Ecu911.AuthService.Interfaces;
using Ecu911.AuthService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecu911.AuthService.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly AuthDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ISystemModuleRepository _systemModuleRepository;
    private readonly IUserSystemRoleRepository _userSystemRoleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IUserSystemScopeRepository _userSystemScopeRepository;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        AuthDbContext context,
        IConfiguration configuration,
        ISystemModuleRepository systemModuleRepository,
        IUserSystemRoleRepository userSystemRoleRepository,
        IPermissionRepository permissionRepository,
        IRolePermissionRepository rolePermissionRepository,
        IUserSystemScopeRepository userSystemScopeRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _context = context;
        _configuration = configuration;
        _systemModuleRepository = systemModuleRepository;
        _userSystemRoleRepository = userSystemRoleRepository;
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _userSystemScopeRepository = userSystemScopeRepository;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto input)
    {
        var normalizedUsername = input.Username.Trim();
        var normalizedEmail = input.Email.Trim().ToLowerInvariant();
        var normalizedCedula = input.Cedula.Trim();

        var existingByUsername = await _userRepository.GetByUsernameAsync(normalizedUsername);
        if (existingByUsername != null)
            throw new Exception("El nombre de usuario ya existe.");

        var existingByEmail = await _userRepository.GetByEmailAsync(normalizedEmail);
        if (existingByEmail != null)
            throw new Exception("Ya existe un usuario con ese correo electrónico.");

        var existingByCedula = await _userRepository.GetByCedulaAsync(normalizedCedula);
        if (existingByCedula != null)
            throw new Exception("Ya existe un usuario con esa cédula.");

        var nombres = input.Nombres.Trim();
        var apellidos = input.Apellidos.Trim();

        var user = new User
        {
            Username = normalizedUsername,
            PasswordHash = PasswordHelper.HashPassword(input.Password),

            Nombres = nombres,
            Apellidos = apellidos,
            Cedula = normalizedCedula,

            Email = normalizedEmail,
            Telefono = input.Telefono.Trim(),

            Cargo = input.Cargo.Trim(),

            ProvinciaId = input.ProvinciaId,
            CantonId = input.CantonId,
            CentroZonalId = input.CentroZonalId,

            IsActive = true,
            OrganizationalUnitId = input.OrganizationalUnitId,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _userRepository.AddAsync(user);
        return MapUserDto(created);
    }

    public async Task<UserDto?> UpdateUserAsync(Guid userId, UpdateUserDto input)
    {
        var user = await _context.Users
            .Include(u => u.Provincia)
            .Include(u => u.Canton)
            .Include(u => u.CentroZonal)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserSystemRoles)
                .ThenInclude(usr => usr.SystemModule)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return null;

        var normalizedUsername = input.Username.Trim();
        var normalizedEmail = input.Email.Trim().ToLowerInvariant();
        var normalizedCedula = input.Cedula.Trim();

        var existingWithSameUsername = await _userRepository.GetByUsernameAsync(normalizedUsername);
        if (existingWithSameUsername != null && existingWithSameUsername.Id != userId)
            throw new Exception("Ya existe otro usuario con ese nombre de usuario.");

        var existingWithSameEmail = await _userRepository.GetByEmailAsync(normalizedEmail);
        if (existingWithSameEmail != null && existingWithSameEmail.Id != userId)
            throw new Exception("Ya existe otro usuario con ese correo electrónico.");

        var existingWithSameCedula = await _userRepository.GetByCedulaAsync(normalizedCedula);
        if (existingWithSameCedula != null && existingWithSameCedula.Id != userId)
            throw new Exception("Ya existe otro usuario con esa cédula.");

        var nombres = input.Nombres.Trim();
        var apellidos = input.Apellidos.Trim();

        user.Username = normalizedUsername;

        user.Nombres = nombres;
        user.Apellidos = apellidos;
        user.Cedula = normalizedCedula;

        user.Email = normalizedEmail;
        user.Telefono = input.Telefono.Trim();

        user.Cargo = input.Cargo.Trim();

        user.ProvinciaId = input.ProvinciaId;
        user.CantonId = input.CantonId;
        user.CentroZonalId = input.CentroZonalId;

        user.IsActive = input.IsActive;
        user.OrganizationalUnitId = input.OrganizationalUnitId;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapUserDto(user);
    }

    public async Task<Role> CreateRoleAsync(CreateRoleDto input)
    {
        var existing = await _roleRepository.GetByNameAsync(input.Name);
        if (existing != null)
            throw new Exception("El rol ya existe.");

        var role = new Role
        {
            Name = input.Name,
            Description = input.Description
        };

        return await _roleRepository.AddAsync(role);
    }

    public async Task<List<RoleDto>> GetRolesAsync()
    {
        var roles = await _roleRepository.GetAllAsync();

        return roles.Select(x => new RoleDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description
        }).ToList();
    }

    public async Task AssignRoleAsync(Guid userId, Guid roleId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("Usuario no encontrado.");

        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            throw new Exception("Rol no encontrado.");

        var exists = await _context.UserRoles.AnyAsync(x => x.UserId == userId && x.RoleId == roleId);
        if (exists)
            throw new Exception("El usuario ya tiene ese rol.");

        _context.UserRoles.Add(new UserRole
        {
            UserId = userId,
            RoleId = roleId
        });

        await _context.SaveChangesAsync();
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapUserDto).ToList();
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto input)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserSystemRoles)
                .ThenInclude(usr => usr.Role)
            .Include(u => u.UserSystemRoles)
                .ThenInclude(usr => usr.SystemModule)
            .FirstOrDefaultAsync(u => u.Username == input.Username);

        if (user == null || !user.IsActive)
            return null;

        if (!PasswordHelper.VerifyPassword(input.Password, user.PasswordHash))
            return null;

        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return BuildBaseLoginResponse(user);
    }

    public async Task<LoginResponseDto> SelectSystemAsync(Guid userId, SelectSystemDto input)
    {
        if (string.IsNullOrWhiteSpace(input.SystemCode))
            throw new Exception("El código del sistema es obligatorio.");

        var normalizedSystemCode = input.SystemCode.Trim().ToUpperInvariant();

        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserSystemRoles)
                .ThenInclude(usr => usr.Role)
            .Include(u => u.UserSystemRoles)
                .ThenInclude(usr => usr.SystemModule)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || !user.IsActive)
            throw new Exception("Usuario no encontrado o inactivo.");

        return await BuildScopedLoginResponseAsync(user, normalizedSystemCode);
    }

    public async Task<SystemModuleDto> CreateSystemModuleAsync(CreateSystemModuleDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
            throw new Exception("El código del sistema es obligatorio.");

        if (string.IsNullOrWhiteSpace(input.Name))
            throw new Exception("El nombre del sistema es obligatorio.");

        var normalizedCode = input.Code.Trim().ToUpperInvariant();

        var existing = await _systemModuleRepository.GetByCodeAsync(normalizedCode);
        if (existing != null)
            throw new Exception("Ya existe un sistema con ese código.");

        var entity = new SystemModule
        {
            Code = normalizedCode,
            Name = input.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(input.Description)
                ? null
                : input.Description.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _systemModuleRepository.AddAsync(entity);

        return new SystemModuleDto
        {
            Id = created.Id,
            Code = created.Code,
            Name = created.Name,
            Description = created.Description,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt
        };
    }

    public async Task<List<SystemModuleDto>> GetSystemModulesAsync()
    {
        var systems = await _systemModuleRepository.GetAllAsync();

        return systems.Select(x => new SystemModuleDto
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt
        }).ToList();
    }

    public async Task AssignUserSystemRoleAsync(AssignUserSystemRoleDto input)
    {
        var user = await _userRepository.GetByIdAsync(input.UserId);
        if (user == null)
            throw new Exception("Usuario no encontrado.");

        var role = await _roleRepository.GetByIdAsync(input.RoleId);
        if (role == null)
            throw new Exception("Rol no encontrado.");

        var system = await _systemModuleRepository.GetByIdAsync(input.SystemModuleId);
        if (system == null || !system.IsActive)
            throw new Exception("Sistema no encontrado o inactivo.");

        var exists = await _userSystemRoleRepository.ExistsAsync(input.UserId, input.RoleId, input.SystemModuleId);
        if (exists)
            throw new Exception("El usuario ya tiene ese rol asignado para ese sistema.");

        await _userSystemRoleRepository.AddAsync(new UserSystemRole
        {
            UserId = input.UserId,
            RoleId = input.RoleId,
            SystemModuleId = input.SystemModuleId
        });
    }

    public async Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
            throw new Exception("El código del permiso es obligatorio.");

        if (string.IsNullOrWhiteSpace(input.Name))
            throw new Exception("El nombre del permiso es obligatorio.");

        var normalizedCode = input.Code.Trim().ToUpperInvariant();

        var existing = await _permissionRepository.GetByCodeAsync(normalizedCode);
        if (existing != null)
            throw new Exception("Ya existe un permiso con ese código.");

        var entity = new Permission
        {
            Code = normalizedCode,
            Name = input.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _permissionRepository.AddAsync(entity);

        return new PermissionDto
        {
            Id = created.Id,
            Code = created.Code,
            Name = created.Name,
            Description = created.Description,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt
        };
    }

    public async Task<List<PermissionDto>> GetPermissionsAsync()
    {
        var permissions = await _permissionRepository.GetAllAsync();

        return permissions.Select(x => new PermissionDto
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt
        }).ToList();
    }

    public async Task AssignRolePermissionAsync(AssignRolePermissionDto input)
    {
        var role = await _roleRepository.GetByIdAsync(input.RoleId);
        if (role == null)
            throw new Exception("Rol no encontrado.");

        var permission = await _permissionRepository.GetByIdAsync(input.PermissionId);
        if (permission == null || !permission.IsActive)
            throw new Exception("Permiso no encontrado o inactivo.");

        var exists = await _rolePermissionRepository.ExistsAsync(input.RoleId, input.PermissionId);
        if (exists)
            throw new Exception("El rol ya tiene asignado ese permiso.");

        await _rolePermissionRepository.AddAsync(new RolePermission
        {
            RoleId = input.RoleId,
            PermissionId = input.PermissionId
        });
    }

    public async Task AssignUserSystemScopeAsync(AssignUserSystemScopeDto input)
    {
        var user = await _userRepository.GetByIdAsync(input.UserId);
        if (user == null)
            throw new Exception("Usuario no encontrado.");

        var system = await _systemModuleRepository.GetByIdAsync(input.SystemModuleId);
        if (system == null || !system.IsActive)
            throw new Exception("Sistema no encontrado o inactivo.");

        if (string.IsNullOrWhiteSpace(input.ScopeLevel))
            throw new Exception("El nivel de alcance es obligatorio.");

        var normalizedScope = input.ScopeLevel.Trim().ToUpperInvariant();

        var allowed = new[] { "LOCAL", "JURISDICCION", "NACIONAL", "EXTERNO" };
        if (!allowed.Contains(normalizedScope))
            throw new Exception("El nivel de alcance no es válido.");

        var existing = await _userSystemScopeRepository.GetByUserAndSystemAsync(input.UserId, input.SystemModuleId);

        if (existing == null)
        {
            await _userSystemScopeRepository.AddAsync(new UserSystemScope
            {
                UserId = input.UserId,
                SystemModuleId = input.SystemModuleId,
                ScopeLevel = normalizedScope,
                CenterCode = string.IsNullOrWhiteSpace(input.CenterCode) ? null : input.CenterCode.Trim().ToUpperInvariant(),
                JurisdictionCode = string.IsNullOrWhiteSpace(input.JurisdictionCode) ? null : input.JurisdictionCode.Trim().ToUpperInvariant(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            return;
        }

        existing.ScopeLevel = normalizedScope;
        existing.CenterCode = string.IsNullOrWhiteSpace(input.CenterCode) ? null : input.CenterCode.Trim().ToUpperInvariant();
        existing.JurisdictionCode = string.IsNullOrWhiteSpace(input.JurisdictionCode) ? null : input.JurisdictionCode.Trim().ToUpperInvariant();
        existing.IsActive = true;

        await _userSystemScopeRepository.UpdateAsync(existing);
    }

    public async Task<UserDto?> GetCurrentUserAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserSystemRoles)
                .ThenInclude(usr => usr.SystemModule)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return null;

        return MapUserDto(user);
    }

    private LoginResponseDto BuildBaseLoginResponse(User user)
    {
        var globalRoles = user.UserRoles
            .Where(x => x.Role != null)
            .Select(x => x.Role.Name)
            .Distinct()
            .ToList();

        var availableSystems = user.UserSystemRoles
            .Where(x => x.SystemModule != null && x.SystemModule.IsActive)
            .Select(x => x.SystemModule.Code)
            .Distinct()
            .ToList();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim("fullName", BuildFullName(user.Nombres, user.Apellidos)),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        if (user.OrganizationalUnitId.HasValue)
        {
            claims.Add(new Claim("organizationalUnitId", user.OrganizationalUnitId.Value.ToString()));
        }

        claims.AddRange(globalRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"]!));
        var token = BuildJwtToken(claims, expiration);

        return new LoginResponseDto
        {
            Token = token,
            Expiration = expiration,
            User = BuildLoginUserDto(user, globalRoles, availableSystems, null)
        };
    }

    private async Task<LoginResponseDto> BuildScopedLoginResponseAsync(User user, string systemCode)
    {
        var globalRoles = user.UserRoles
            .Where(x => x.Role != null)
            .Select(x => x.Role.Name)
            .Distinct()
            .ToList();

        var isGlobalAdmin = globalRoles.Contains("ADMIN");

        var availableSystems = user.UserSystemRoles
            .Where(x => x.SystemModule != null && x.SystemModule.IsActive)
            .Select(x => x.SystemModule.Code)
            .Distinct()
            .ToList();

        var currentSystem = await _systemModuleRepository.GetByCodeAsync(systemCode);

        if (currentSystem == null || !currentSystem.IsActive)
            throw new Exception("El sistema especificado no existe o está inactivo.");

        var systemRoles = user.UserSystemRoles
            .Where(x => x.SystemModuleId == currentSystem.Id)
            .Select(x => x.Role.Name)
            .Distinct()
            .ToList();

        if (!isGlobalAdmin && !systemRoles.Any())
            throw new Exception("El usuario no tiene acceso al sistema solicitado.");

        var effectiveRoles = globalRoles
            .Concat(systemRoles)
            .Distinct()
            .ToList();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim("fullName", BuildFullName(user.Nombres, user.Apellidos)),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("system_code", currentSystem.Code),
            new Claim("system_name", currentSystem.Name)
        };

        if (user.OrganizationalUnitId.HasValue)
        {
            claims.Add(new Claim("organizationalUnitId", user.OrganizationalUnitId.Value.ToString()));
        }

        claims.AddRange(effectiveRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"]!));
        var token = BuildJwtToken(claims, expiration);

        return new LoginResponseDto
        {
            Token = token,
            Expiration = expiration,
            User = BuildLoginUserDto(user, effectiveRoles, availableSystems, currentSystem.Code)
        };
    }

    private string BuildJwtToken(List<Claim> claims, DateTime expiration)
    {
        var secretKey = _configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(secretKey))
            throw new Exception("La clave JWT no está configurada.");

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiration,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string BuildFullName(string nombres, string apellidos)
    {
        return $"{nombres} {apellidos}".Trim();
    }

    private UserDto BuildLoginUserDto(
        User user,
        List<string> roles,
        List<string> systems,
        string? currentSystem)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = BuildFullName(user.Nombres, user.Apellidos),
            Nombres = user.Nombres,
            Apellidos = user.Apellidos,
            Cedula = user.Cedula,
            Email = user.Email,
            Telefono = user.Telefono,
            Cargo = user.Cargo,
            ProvinciaId = user.ProvinciaId,
            Provincia = user.Provincia?.Nombre ?? string.Empty,
            CantonId = user.CantonId,
            Canton = user.Canton?.Nombre ?? string.Empty,
            CentroZonalId = user.CentroZonalId,
            CentroZonal = user.CentroZonal?.Nombre ?? string.Empty,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLoginAt = user.LastLoginAt,
            OrganizationalUnitId = user.OrganizationalUnitId,
            Roles = roles,
            Systems = systems,
            CurrentSystem = currentSystem
        };
    }

    private UserDto MapUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = BuildFullName(user.Nombres, user.Apellidos),
            Nombres = user.Nombres,
            Apellidos = user.Apellidos,
            Cedula = user.Cedula,
            Email = user.Email,
            Telefono = user.Telefono,
            Cargo = user.Cargo,
            ProvinciaId = user.ProvinciaId,
            Provincia = user.Provincia?.Nombre ?? string.Empty,
            CantonId = user.CantonId,
            Canton = user.Canton?.Nombre ?? string.Empty,
            CentroZonalId = user.CentroZonalId,
            CentroZonal = user.CentroZonal?.Nombre ?? string.Empty,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLoginAt = user.LastLoginAt,
            OrganizationalUnitId = user.OrganizationalUnitId,
            Roles = user.UserRoles.Where(x => x.Role != null).Select(x => x.Role.Name).Distinct().ToList(),
            Systems = user.UserSystemRoles.Where(x => x.SystemModule != null).Select(x => x.SystemModule.Code).Distinct().ToList()
        };
    }
}
