using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;

namespace BalkalaInfra.Application.Interfaces;

public interface IIdentityManagementService
{
    Task<IdentityUserResponse> Register(RegisterRequest request, CancellationToken ct);
    Task<AuthResponse> Login(LoginRequest request, CancellationToken ct);
    Task<AuthResponse> Refresh(RefreshRequest request, CancellationToken ct);
    Task Logout(int userId, string accessToken, CancellationToken ct);
    Task<IdentityUserResponse> Me(int tenantId, int userId, CancellationToken ct);
    Task ChangePassword(int userId, string currentPassword, string newPassword, CancellationToken ct);
    Task<IReadOnlyList<UserAccountResponse>> GetUsers(CancellationToken ct);
    Task<UserAccountResponse> GetUserAccount(int id, CancellationToken ct);
    Task<IdentityUserResponse> GetUser(int tenantId, int id, CancellationToken ct);
    Task<UserAccountResponse> CreateUser(CreateUserRequest request, CancellationToken ct);
    Task<UserAccountResponse> UpdateUser(int id, UpdateUserRequest request, CancellationToken ct);
    Task UpdateUserStatus(int id, UpdateUserStatusRequest request, CancellationToken ct);
    Task DeleteUser(int id, CancellationToken ct);
    Task<IReadOnlyList<RoleResponse>> GetUserRoles(int id, CancellationToken ct);
    Task AssignRole(int id, AssignRoleRequest request, CancellationToken ct);
    Task RemoveRole(int id, int roleId, int tenantId, CancellationToken ct);
    Task<IReadOnlyList<RoleResponse>> GetRoles(CancellationToken ct);
    Task<RoleResponse> GetRole(int id, CancellationToken ct);
    Task<RoleResponse> CreateRole(CreateRoleRequest request, CancellationToken ct);
    Task<RoleResponse> UpdateRole(int id, UpdateRoleRequest request, CancellationToken ct);
    Task DeleteRole(int id, int tenantId, CancellationToken ct);
    Task<IReadOnlyList<RolePermissionResponse>> GetRolePermissions(int id, CancellationToken ct);
    Task SetRolePermissions(int id, UpdateRolePermissionsRequest request, CancellationToken ct);
    Task<IReadOnlyList<PermissionResponse>> GetPermissions(CancellationToken ct);
    Task<PermissionResponse> GetPermission(int id, CancellationToken ct);
    Task<PermissionResponse> CreatePermission(CreatePermissionRequest request, CancellationToken ct);
    Task<PermissionResponse> UpdatePermission(int id, UpdatePermissionRequest request, CancellationToken ct);
    Task DeletePermission(int id, CancellationToken ct);
}
