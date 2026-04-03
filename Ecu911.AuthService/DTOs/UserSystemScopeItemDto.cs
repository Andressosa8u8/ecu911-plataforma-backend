namespace Ecu911.AuthService.DTOs;

public class UserSystemScopeItemDto
{
    public Guid SystemModuleId { get; set; }
    public string SystemCode { get; set; } = default!;
    public string SystemName { get; set; } = default!;
    public bool SystemIsActive { get; set; }

    public string ScopeLevel { get; set; } = default!;
    public string? CenterCode { get; set; }
    public string? JurisdictionCode { get; set; }
    public bool IsActive { get; set; }
}