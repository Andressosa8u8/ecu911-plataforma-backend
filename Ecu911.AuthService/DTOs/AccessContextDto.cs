namespace Ecu911.AuthService.DTOs;

public class AccessContextDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public bool IsGlobalAdmin { get; set; }
    public string? CurrentSystemCode { get; set; }
    public string? CurrentSystemName { get; set; }
    public List<AccessContextRoleDto> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
    public List<AccessContextScopeDto> Scopes { get; set; } = new();
}

public class AccessContextRoleDto
{
    public Guid RoleId { get; set; }
    public string Name { get; set; } = default!;
    public string RoleType { get; set; } = default!;
    public bool IsActive { get; set; }
}

public class AccessContextScopeDto
{
    public string ScopeLevel { get; set; } = default!;
    public string? CenterCode { get; set; }
    public string? JurisdictionCode { get; set; }
    public bool IsActive { get; set; }
}
