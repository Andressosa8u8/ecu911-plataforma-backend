using Ecu911.AuthService.DTOs;
using Ecu911.AuthService.Models;

namespace Ecu911.AuthService.Interfaces;

public interface IAuthService
{
    Task<UserDto> CreateUserAsync(CreateUserDto input);
    Task<UserDto?> UpdateUserAsync(Guid userId, UpdateUserDto input);
    Task<List<UserDto>> GetUsersAsync();
    Task<UserAccessProfileDto?> GetUserAccessProfileAsync(Guid userId);

    Task<Role> CreateRoleAsync(CreateRoleDto input);
    Task<List<RoleDto>> GetRolesAsync();
    Task<RoleDto?> UpdateRoleAsync(Guid roleId, UpdateRoleDto input);
    Task<RoleDto?> ChangeRoleStatusAsync(Guid roleId, bool isActive);
    Task AssignRoleAsync(Guid userId, Guid roleId);
    Task RemoveRoleAsync(Guid userId, Guid roleId);

    Task<UserDto?> GetCurrentUserAsync(Guid userId);
    Task<LoginResponseDto?> LoginAsync(LoginDto input);
    Task<LoginResponseDto> SelectSystemAsync(Guid userId, SelectSystemDto input);

    Task<SystemModuleDto> CreateSystemModuleAsync(CreateSystemModuleDto input);
    Task<List<SystemModuleDto>> GetSystemModulesAsync();
    Task<SystemModuleDto?> UpdateSystemModuleAsync(Guid systemModuleId, UpdateSystemModuleDto input);
    Task<SystemModuleDto?> ChangeSystemModuleStatusAsync(Guid systemModuleId, bool isActive);
    Task AssignUserSystemRoleAsync(AssignUserSystemRoleDto input);
    Task RemoveUserSystemRoleAsync(RemoveUserSystemRoleDto input);

    Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto input);
    Task<List<PermissionDto>> GetPermissionsAsync();
    Task<PermissionDto?> UpdatePermissionAsync(Guid permissionId, UpdatePermissionDto input);
    Task AssignRolePermissionAsync(AssignRolePermissionDto input);
    Task<List<RolePermissionItemDto>> GetRolePermissionsAsync(Guid roleId);
    Task RemoveRolePermissionAsync(Guid roleId, Guid permissionId);

    Task AssignUserSystemScopeAsync(AssignUserSystemScopeDto input);
    Task RemoveUserSystemScopeAsync(RemoveUserSystemScopeDto input);
}