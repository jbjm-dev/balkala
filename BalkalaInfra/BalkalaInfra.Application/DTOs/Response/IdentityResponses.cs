namespace BalkalaInfra.Application.DTOs.Response;

public sealed record IdentityUserResponse(int UserId, int TenantUserId, string Email, string? Mobile, string DisplayName, string GlobalStatus, string MembershipStatus, IReadOnlyList<string> Roles);
public sealed record UserAccountResponse(int UserId, string Email, string? Mobile, string? IdentityProvider, string? GlobalStatus, DateTime? LastLoginAt);
public sealed record AuthResponse(string AccessToken, DateTime AccessTokenExpiresAt, string RefreshToken, DateTime RefreshTokenExpiresAt, IdentityUserResponse User);
public sealed record RoleResponse(int RoleId, string RoleCode, string RoleName, string RoleType, bool IsSystemRole);
public sealed record PermissionResponse(int PermissionId, string PermissionCode, string ModuleCode, string ActionCode, string? Description);
public sealed record RolePermissionResponse(int PermissionId, string PermissionCode, string AccessScope);
public sealed record ApiMessageResponse(bool Success, string Message);
