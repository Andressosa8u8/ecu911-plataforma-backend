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
        var normalizedName = input.Name.Trim().ToUpperInvariant();

        var existing = await _roleRepository.GetByNameAsync(normalizedName);
        if (existing != null)
            throw new Exception("El rol ya existe.");

        var normalizedRoleType = string.IsNullOrWhiteSpace(input.RoleType)
            ? "FUNCIONAL"
            : input.RoleType.Trim().ToUpperInvariant();

        var allowedRoleTypes = new[] { "GLOBAL", "FUNCIONAL" };
        if (!allowedRoleTypes.Contains(normalizedRoleType))
            throw new Exception("El tipo de rol no es válido.");

        var role = new Role
        {
            Name = normalizedName,
            Description = string.IsNullOrWhiteSpace(input.Description) ? string.Empty : input.Description.Trim(),
            RoleType = normalizedRoleType,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
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
            Description = x.Description,
            RoleType = x.RoleType,
            IsActive = x.IsActive,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }).ToList();
    }

    public async Task<RoleDto?> UpdateRoleAsync(Guid roleId, UpdateRoleDto input)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            return null;

        var normalizedName = input.Name.Trim().ToUpperInvariant();

        var existing = await _roleRepository.GetByNameAsync(normalizedName);
        if (existing != null && existing.Id != roleId)
            throw new Exception("Ya existe otro rol con ese nombre.");

        var normalizedRoleType = string.IsNullOrWhiteSpace(input.RoleType)
            ? "FUNCIONAL"
            : input.RoleType.Trim().ToUpperInvariant();

        var allowedRoleTypes = new[] { "GLOBAL", "FUNCIONAL" };
        if (!allowedRoleTypes.Contains(normalizedRoleType))
            throw new Exception("El tipo de rol no es válido.");

        role.Name = normalizedName;
        role.Description = string.IsNullOrWhiteSpace(input.Description) ? string.Empty : input.Description.Trim();
        role.RoleType = normalizedRoleType;
        role.UpdatedAt = DateTime.UtcNow;

        await _roleRepository.UpdateAsync(role);

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            RoleType = role.RoleType,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt
        };
    }

    public async Task<RoleDto?> ChangeRoleStatusAsync(Guid roleId, bool isActive)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            return null;

        role.IsActive = isActive;
        role.UpdatedAt = DateTime.UtcNow;

        await _roleRepository.UpdateAsync(role);

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt
        };
    }

    public async Task AssignRoleAsync(Guid userId, Guid roleId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("Usuario no encontrado.");

        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            throw new Exception("Rol no encontrado.");

        if (!role.IsActive)
            throw new Exception("No se puede asignar un rol inactivo.");

        var exists = await _context.UserRoles.AnyAsync(x => x.UserId == userId && x.RoleId == roleId);
        if (exists)
            throw new Exception("El usuario ya tiene ese rol.");

        if (role.RoleType != "GLOBAL")
            throw new Exception("Solo los roles globales pueden asignarse como rol general del usuario.");

        _context.UserRoles.Add(new UserRole
        {
            UserId = userId,
            RoleId = roleId
        });

        await _context.SaveChangesAsync();
    }

    public async Task RemoveRoleAsync(Guid userId, Guid roleId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("Usuario no encontrado.");

        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            throw new Exception("Rol no encontrado.");

        var entity = await _context.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId);
        if (entity == null)
            throw new Exception("El usuario no tiene asignado ese rol.");

        _context.UserRoles.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapUserDto).ToList();
    }

    public async Task<UserAccessProfileDto?> GetUserAccessProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return null;

        var systemRoles = await _userSystemRoleRepository.GetByUserIdAsync(userId);
        var systemScopes = await _userSystemScopeRepository.GetByUserIdAsync(userId);

        return new UserAccessProfileDto
        {
            UserId = user.Id,
            Username = user.Username,
            Nombres = user.Nombres,
            Apellidos = user.Apellidos,
            Cedula = user.Cedula,
            Email = user.Email,
            Telefono = user.Telefono,
            Cargo = user.Cargo,
            ProvinciaId = user.ProvinciaId,
            ProvinciaNombre = user.Provincia?.Nombre,
            CantonId = user.CantonId,
            CantonNombre = user.Canton?.Nombre,
            CentroZonalId = user.CentroZonalId,
            CentroZonalNombre = user.CentroZonal?.Nombre,
            IsActive = user.IsActive,

            GlobalRoles = user.UserRoles
                .OrderBy(x => x.Role.Name)
                .Select(x => new UserGlobalRoleItemDto
                {
                    RoleId = x.RoleId,
                    RoleName = x.Role.Name,
                    RoleDescription = x.Role.Description,
                    RoleType = x.Role.RoleType,
                    IsActive = x.Role.IsActive
                })
                .ToList(),

            SystemRoles = systemRoles
                .Select(x => new UserSystemRoleItemDto
                {
                    SystemModuleId = x.SystemModuleId,
                    SystemCode = x.SystemModule.Code,
                    SystemName = x.SystemModule.Name,
                    SystemIsActive = x.SystemModule.IsActive,
                    RoleId = x.RoleId,
                    RoleName = x.Role.Name,
                    RoleDescription = x.Role.Description,
                    RoleType = x.Role.RoleType,
                    RoleIsActive = x.Role.IsActive
                })
                .ToList(),

            SystemScopes = systemScopes
                .Select(x => new UserSystemScopeItemDto
                {
                    SystemModuleId = x.SystemModuleId,
                    SystemCode = x.SystemModule.Code,
                    SystemName = x.SystemModule.Name,
                    SystemIsActive = x.SystemModule.IsActive,
                    ScopeLevel = x.ScopeLevel,
                    CenterCode = x.CenterCode,
                    JurisdictionCode = x.JurisdictionCode,
                    IsActive = x.IsActive
                })
                .ToList()
        };
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

    public async Task<SystemModuleDto?> UpdateSystemModuleAsync(Guid systemModuleId, UpdateSystemModuleDto input)
    {
        var entity = await _systemModuleRepository.GetByIdAsync(systemModuleId);
        if (entity == null)
            return null;

        if (string.IsNullOrWhiteSpace(input.Code))
            throw new Exception("El código del sistema es obligatorio.");

        if (string.IsNullOrWhiteSpace(input.Name))
            throw new Exception("El nombre del sistema es obligatorio.");

        var normalizedCode = input.Code.Trim().ToUpperInvariant();

        var existing = await _systemModuleRepository.GetByCodeAsync(normalizedCode);
        if (existing != null && existing.Id != systemModuleId)
            throw new Exception("Ya existe otro sistema con ese código.");

        entity.Code = normalizedCode;
        entity.Name = input.Name.Trim();
        entity.Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim();

        await _systemModuleRepository.UpdateAsync(entity);

        return new SystemModuleDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task<SystemModuleDto?> ChangeSystemModuleStatusAsync(Guid systemModuleId, bool isActive)
    {
        var entity = await _systemModuleRepository.GetByIdAsync(systemModuleId);
        if (entity == null)
            return null;

        entity.IsActive = isActive;

        await _systemModuleRepository.UpdateAsync(entity);

        return new SystemModuleDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task AssignUserSystemRoleAsync(AssignUserSystemRoleDto input)
    {
        var user = await _userRepository.GetByIdAsync(input.UserId);
        if (user == null)
            throw new Exception("Usuario no encontrado.");

        var role = await _roleRepository.GetByIdAsync(input.RoleId);
        if (role == null)
            throw new Exception("Rol no encontrado.");

        if (!role.IsActive)
            throw new Exception("No se puede asignar un rol inactivo.");

        var system = await _systemModuleRepository.GetByIdAsync(input.SystemModuleId);
        if (system == null || !system.IsActive)
            throw new Exception("Sistema no encontrado o inactivo.");

        var exists = await _userSystemRoleRepository.ExistsAsync(input.UserId, input.RoleId, input.SystemModuleId);
        if (exists)
            throw new Exception("El usuario ya tiene ese rol asignado para ese sistema.");

        if (role.RoleType != "FUNCIONAL")
            throw new Exception("Solo los roles funcionales pueden asignarse dentro de un sistema.");

        await _userSystemRoleRepository.AddAsync(new UserSystemRole
        {
            UserId = input.UserId,
            RoleId = input.RoleId,
            SystemModuleId = input.SystemModuleId
        });
    }

    public async Task RemoveUserSystemRoleAsync(RemoveUserSystemRoleDto input)
    {
        var user = await _userRepository.GetByIdAsync(input.UserId);
        if (user == null)
            throw new Exception("Usuario no encontrado.");

        var role = await _roleRepository.GetByIdAsync(input.RoleId);
        if (role == null)
            throw new Exception("Rol no encontrado.");

        var system = await _systemModuleRepository.GetByIdAsync(input.SystemModuleId);
        if (system == null)
            throw new Exception("Sistema no encontrado.");

        var entity = await _userSystemRoleRepository.GetAsync(input.UserId, input.RoleId, input.SystemModuleId);
        if (entity == null)
            throw new Exception("El usuario no tiene ese rol asignado en el sistema indicado.");

        await _userSystemRoleRepository.RemoveAsync(entity);
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

    public async Task<PermissionDto?> UpdatePermissionAsync(Guid permissionId, UpdatePermissionDto input)
    {
        var permission = await _permissionRepository.GetByIdAsync(permissionId);
        if (permission == null)
            return null;

        if (string.IsNullOrWhiteSpace(input.Code))
            throw new Exception("El código del permiso es obligatorio.");

        if (string.IsNullOrWhiteSpace(input.Name))
            throw new Exception("El nombre del permiso es obligatorio.");

        var normalizedCode = input.Code.Trim().ToUpperInvariant();

        var existingWithSameCode = await _permissionRepository.GetByCodeAsync(normalizedCode);
        if (existingWithSameCode != null && existingWithSameCode.Id != permissionId)
            throw new Exception("Ya existe otro permiso con ese código.");

        permission.Code = normalizedCode;
        permission.Name = input.Name.Trim().ToUpperInvariant();
        permission.Description = input.Description?.Trim() ?? string.Empty;
        permission.IsActive = input.IsActive;

        await _permissionRepository.UpdateAsync(permission);

        return new PermissionDto
        {
            Id = permission.Id,
            Code = permission.Code,
            Name = permission.Name,
            Description = permission.Description,
            IsActive = permission.IsActive,
            CreatedAt = permission.CreatedAt
        };
    }

    public async Task<List<RolePermissionItemDto>> GetRolePermissionsAsync(Guid roleId)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            throw new Exception("Rol no encontrado.");

        var items = await _rolePermissionRepository.GetByRoleIdAsync(roleId);

        return items
            .Where(x => x.Permission != null)
            .Select(x => new RolePermissionItemDto
            {
                PermissionId = x.PermissionId,
                Code = x.Permission!.Code,
                Name = x.Permission.Name,
                Description = x.Permission.Description,
                IsActive = x.Permission.IsActive
            })
            .OrderBy(x => x.Code)
            .ToList();
    }

    public async Task RemoveRolePermissionAsync(Guid roleId, Guid permissionId)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
            throw new Exception("Rol no encontrado.");

        var relation = await _rolePermissionRepository.GetAsync(roleId, permissionId);
        if (relation == null)
            throw new Exception("El permiso no está asignado a ese rol.");

        await _rolePermissionRepository.RemoveAsync(relation);
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

        var centerCode = string.IsNullOrWhiteSpace(input.CenterCode)
            ? null
            : input.CenterCode.Trim().ToUpperInvariant();

        var jurisdictionCode = string.IsNullOrWhiteSpace(input.JurisdictionCode)
            ? null
            : input.JurisdictionCode.Trim().ToUpperInvariant();

        var existing = await _userSystemScopeRepository.GetByUserAndSystemAsync(input.UserId, input.SystemModuleId);

        if (existing == null)
        {
            await _userSystemScopeRepository.AddAsync(new UserSystemScope
            {
                UserId = input.UserId,
                SystemModuleId = input.SystemModuleId,
                ScopeLevel = normalizedScope,
                CenterCode = centerCode,
                JurisdictionCode = jurisdictionCode,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            return;
        }

        existing.ScopeLevel = normalizedScope;
        existing.CenterCode = centerCode;
        existing.JurisdictionCode = jurisdictionCode;
        existing.IsActive = true;

        await _userSystemScopeRepository.UpdateAsync(existing);
    }

    public async Task RemoveUserSystemScopeAsync(RemoveUserSystemScopeDto input)
    {
        var user = await _userRepository.GetByIdAsync(input.UserId);
        if (user == null)
            throw new Exception("Usuario no encontrado.");

        var system = await _systemModuleRepository.GetByIdAsync(input.SystemModuleId);
        if (system == null)
            throw new Exception("Sistema no encontrado.");

        if (string.IsNullOrWhiteSpace(input.ScopeLevel))
            throw new Exception("El nivel de alcance es obligatorio.");

        var normalizedScope = input.ScopeLevel.Trim().ToUpperInvariant();

        var centerCode = string.IsNullOrWhiteSpace(input.CenterCode)
            ? null
            : input.CenterCode.Trim().ToUpperInvariant();

        var jurisdictionCode = string.IsNullOrWhiteSpace(input.JurisdictionCode)
            ? null
            : input.JurisdictionCode.Trim().ToUpperInvariant();

        var entity = await _userSystemScopeRepository.GetExactAsync(
            input.UserId,
            input.SystemModuleId,
            normalizedScope,
            centerCode,
            jurisdictionCode);

        if (entity == null)
            throw new Exception("El alcance indicado no está asignado al usuario en ese sistema.");

        await _userSystemScopeRepository.RemoveAsync(entity);
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

    public async Task<AccessContextDto> GetAccessContextAsync(Guid userId, string? currentSystemCode)
    {
        var normalizedSystemCode = string.IsNullOrWhiteSpace(currentSystemCode)
            ? null
            : currentSystemCode.Trim().ToUpperInvariant();

        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserSystemRoles)
                .ThenInclude(usr => usr.Role)
            .Include(u => u.UserSystemRoles)
                .ThenInclude(usr => usr.SystemModule)
            .Include(u => u.UserSystemScopes)
                .ThenInclude(uss => uss.SystemModule)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || !user.IsActive)
            throw new Exception("Usuario no encontrado o inactivo.");

        var globalRoles = user.UserRoles
            .Where(x => x.Role != null && x.Role.IsActive)
            .Select(x => x.Role)
            .DistinctBy(x => x.Id)
            .ToList();

        var systemRoles = user.UserSystemRoles
            .Where(x =>
                x.Role != null &&
                x.Role.IsActive &&
                x.SystemModule != null &&
                x.SystemModule.IsActive &&
                (normalizedSystemCode == null || x.SystemModule.Code == normalizedSystemCode))
            .Select(x => x.Role)
            .DistinctBy(x => x.Id)
            .ToList();

        var effectiveRoles = globalRoles
            .Concat(systemRoles)
            .DistinctBy(x => x.Id)
            .ToList();

        var isGlobalAdmin = globalRoles.Any(r => string.Equals(r.RoleType, "GLOBAL", StringComparison.OrdinalIgnoreCase))
            || globalRoles.Any(r => string.Equals(r.Name, "ADMIN", StringComparison.OrdinalIgnoreCase));

        var roleIds = effectiveRoles.Select(r => r.Id).ToList();

        var permissions = roleIds.Count == 0
            ? new List<string>()
            : await _context.Set<RolePermission>()
                .Include(rp => rp.Permission)
                .Where(rp => roleIds.Contains(rp.RoleId) && rp.Permission != null && rp.Permission.IsActive)
                .Select(rp => rp.Permission.Code)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

        var scopes = user.UserSystemScopes
            .Where(x =>
                x.IsActive &&
                x.SystemModule != null &&
                x.SystemModule.IsActive &&
                (normalizedSystemCode == null || x.SystemModule.Code == normalizedSystemCode))
            .Select(x => new AccessContextScopeDto
            {
                ScopeLevel = x.ScopeLevel,
                CenterCode = x.CenterCode,
                JurisdictionCode = x.JurisdictionCode,
                IsActive = x.IsActive
            })
            .ToList();

        var currentSystem = user.UserSystemRoles
            .Where(x => x.SystemModule != null && (normalizedSystemCode == null || x.SystemModule.Code == normalizedSystemCode))
            .Select(x => x.SystemModule)
            .FirstOrDefault();

        return new AccessContextDto
        {
            UserId = user.Id,
            Username = user.Username,
            FullName = BuildFullName(user.Nombres, user.Apellidos),
            IsGlobalAdmin = isGlobalAdmin,
            CurrentSystemCode = currentSystem?.Code ?? normalizedSystemCode,
            CurrentSystemName = currentSystem?.Name,
            Roles = effectiveRoles
                .Select(r => new AccessContextRoleDto
                {
                    RoleId = r.Id,
                    Name = r.Name,
                    RoleType = r.RoleType,
                    IsActive = r.IsActive
                })
                .OrderBy(r => r.RoleType)
                .ThenBy(r => r.Name)
                .ToList(),
            Permissions = permissions,
            Scopes = scopes
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
