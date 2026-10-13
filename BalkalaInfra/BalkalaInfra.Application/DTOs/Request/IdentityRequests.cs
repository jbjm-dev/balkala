using System.ComponentModel.DataAnnotations;

namespace BalkalaInfra.Application.DTOs.Request;

public sealed record RegisterRequest(
    [Required, EmailAddress, MaxLength(150)] string Email,
    [Required, MinLength(8), MaxLength(200)] string Password,
    [Required, MaxLength(150)] string DisplayName,
    [MaxLength(20)] string? Mobile);
public sealed record LoginRequest([Required, EmailAddress, MaxLength(150)] string Email, [Required] string Password);
public sealed record RefreshRequest([Required] string RefreshToken);
public sealed record ChangePasswordRequest([Required] string CurrentPassword, [Required, MinLength(8), MaxLength(200)] string NewPassword);
public sealed record CreateUserRequest([Required, EmailAddress, MaxLength(150)] string Email, [Required, MinLength(8), MaxLength(200)] string Password, [MaxLength(20)] string? Mobile, [MaxLength(20)] string? IdentityProvider, [Required] string GlobalStatus = "Active");
public sealed record UpdateUserRequest([Required, EmailAddress, MaxLength(150)] string Email, [MaxLength(20)] string? Mobile, [MaxLength(20)] string? IdentityProvider);
public sealed record UpdateUserStatusRequest([Required] string Status);
public sealed record AssignRoleRequest([Range(1, int.MaxValue)] int TenantId, [Required] int RoleId, int? OrganizationId, int? ProjectId, DateTime? EffectiveFrom, DateTime? EffectiveTo);
public sealed record RemoveUserRoleRequest([Range(1, int.MaxValue)] int TenantId);
public sealed record CreateRoleRequest([Range(1, int.MaxValue)] int TenantId, [Required, MaxLength(50)] string RoleCode, [Required, MaxLength(100)] string RoleName);
public sealed record UpdateRoleRequest([Range(1, int.MaxValue)] int TenantId, [Required, MaxLength(100)] string RoleName);
public sealed record DeleteRoleRequest([Range(1, int.MaxValue)] int TenantId);
public sealed record RolePermissionAssignment([Required] int PermissionId, [Required] string AccessScope);
public sealed record UpdateRolePermissionsRequest([Range(1, int.MaxValue)] int TenantId, [Required] IReadOnlyList<RolePermissionAssignment> Permissions);
public sealed record CreatePermissionRequest([Required, MaxLength(100)] string PermissionCode, [Required, MaxLength(100)] string ModuleCode, [Required, MaxLength(50)] string ActionCode, string? Description);
public sealed record UpdatePermissionRequest([Required, MaxLength(100)] string ModuleCode, [Required, MaxLength(50)] string ActionCode, string? Description);
