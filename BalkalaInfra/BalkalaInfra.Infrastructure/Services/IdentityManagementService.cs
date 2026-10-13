using System.Security.Cryptography;
using System.Text;
using BalkalaInfra.Application.DTOs.Request;
using BalkalaInfra.Application.DTOs.Response;
using BalkalaInfra.Application.Exceptions;
using BalkalaInfra.Application.Interfaces;
using BalkalaInfra.Domain.Entities;
using BalkalaInfra.Infrastructure.Data;
using BalkalaInfra.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace BalkalaInfra.Infrastructure.Services;

/// <summary>
/// Identity CRUD, JWT access-token, and rotating refresh-token operations.
/// </summary>
public sealed class IdentityManagementService(AppDbContext db, JwtTokenService jwtTokenService) : IIdentityManagementService
{
    private const string DefaultTenantCode = "BALKALA";

    public async Task<IdentityUserResponse> Register(RegisterRequest request, CancellationToken ct)
    {
        var tenant = await db.Tenants.SingleOrDefaultAsync(x => x.TenantCode == DefaultTenantCode, ct)
            ?? throw NotFound("Tenant not found.");
        if (await db.Users.AnyAsync(x => x.Email == request.Email.Trim().ToLowerInvariant(), ct))
            throw Conflict("An account with this email already exists.");

        var user = new User
        {
            Email = request.Email.Trim().ToLowerInvariant(), Mobile = request.Mobile,
            PasswordHash = HashPassword(request.Password), GlobalStatus = "Active"
        };
        var membership = new TenantUser
        {
            TenantId = tenant.TenantId, UserId = user.UserId,
            DisplayName = request.DisplayName.Trim(), Status = "Active", User = user, Tenant = tenant
        };
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        db.Users.Add(user);
        db.TenantUsers.Add(membership);
        await db.SaveChangesAsync(ct);

        // The membership's generated key is available only after the first save.
        await AssignDefaultEmployeeRole(tenant.TenantId, membership.TenantUserId, ct);
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return await MapUser(membership, ct);
    }

    public async Task<AuthResponse> Login(LoginRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.SingleOrDefaultAsync(x => x.Email == email, ct);
        if (user is null || !VerifyPassword(request.Password, user.PasswordHash)) throw Unauthorized("Email or password is incorrect.");
        if (user.GlobalStatus != "Active") throw Forbidden("This user account is inactive.");

        var memberships = await db.TenantUsers.Include(x => x.Tenant)
            .Where(x => x.UserId == user.UserId && x.Status == "Active" && x.Tenant.Status == "Active")
            .OrderByDescending(x => x.TenantUserRoles.Any(r => r.Role.RoleCode == "Admin"
                && (r.EffectiveFrom == null || r.EffectiveFrom <= DateTime.UtcNow)
                && (r.EffectiveTo == null || r.EffectiveTo > DateTime.UtcNow)))
            .ThenByDescending(x => x.Tenant.TenantCode == DefaultTenantCode)
            .ThenBy(x => x.Tenant.TenantId).ToListAsync(ct);
        if (memberships.Count == 0) throw Forbidden("This user has no active tenant memberships.");
        user.LastLoginAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        var member = memberships[0];
        return await CreateSession(user, member.Tenant, member, null, ct);
    }

    public async Task<AuthResponse> Refresh(RefreshRequest request, CancellationToken ct)
    {
        var hash = TokenHash(request.RefreshToken);
        var session = await db.UserSessions.Include(x => x.User).Include(x => x.Tenant)
            .SingleOrDefaultAsync(x => x.RefreshTokenHash == hash && x.RevokedAt == null && x.RefreshExpiresAt > DateTime.UtcNow, ct)
            ?? throw Unauthorized("Refresh token is expired or invalid.");
        if (session.TenantId is null || session.User.GlobalStatus != "Active" || session.Tenant?.Status != "Active") throw Forbidden("The user or tenant is inactive.");

        var user = session.User;
        var membership = await db.TenantUsers.Include(x => x.User).SingleOrDefaultAsync(x => x.UserId == user.UserId && x.TenantId == session.TenantId && x.Status == "Active", ct)
            ?? throw Forbidden("The tenant membership is inactive.");
        var mappedUser = await MapUser(membership, ct);
        var jwt = jwtTokenService.CreateAccessToken(user, membership, mappedUser.Roles);
        var accessToken = jwt.Token;
        var refreshToken = NewToken();
        session.RefreshTokenHash = TokenHash(refreshToken);
        session.SessionTokenHash = TokenHash(accessToken);
        session.ExpiresAt = jwt.ExpiresAt;
        session.RefreshExpiresAt = DateTime.UtcNow.AddDays(30);
        await db.SaveChangesAsync(ct);
        return new AuthResponse(accessToken, session.ExpiresAt, refreshToken, session.RefreshExpiresAt, mappedUser);
    }

    public async Task Logout(int userId, string accessToken, CancellationToken ct)
    {
        var session = await db.UserSessions.SingleOrDefaultAsync(x => x.UserId == userId && x.SessionTokenHash == TokenHash(accessToken) && x.RevokedAt == null, ct)
            ?? throw NotFound("Session not found.");
        session.RevokedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    public Task<IdentityUserResponse> Me(int tenantId, int userId, CancellationToken ct) => GetUser(tenantId, userId, ct);

    public async Task ChangePassword(int userId, string currentPassword, string newPassword, CancellationToken ct)
    {
        var user = await db.Users.FindAsync([userId], ct) ?? throw NotFound("User not found.");
        if (!VerifyPassword(currentPassword, user.PasswordHash)) throw Unauthorized("Current password is incorrect.");
        user.PasswordHash = HashPassword(newPassword);
        
        var sessions = await db.UserSessions.Where(x => x.UserId == userId && x.RevokedAt == null).ToListAsync(ct);
        foreach (var session in sessions) session.RevokedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<UserAccountResponse>> GetUsers(CancellationToken ct)
    {
        return await db.Users.AsNoTracking()
            .OrderBy(x => x.UserId)
            .Select(x => new UserAccountResponse(
                x.UserId,
                x.Email,
                x.Mobile,
                x.IdentityProvider,
                x.GlobalStatus,
                x.LastLoginAt))
            .ToListAsync(ct);
    }

    public async Task<UserAccountResponse> GetUserAccount(int id, CancellationToken ct) =>
        await db.Users.AsNoTracking().Where(x => x.UserId == id)
            .Select(x => new UserAccountResponse(x.UserId, x.Email, x.Mobile, x.IdentityProvider, x.GlobalStatus, x.LastLoginAt))
            .SingleOrDefaultAsync(ct) ?? throw NotFound("User not found.");

    public async Task<IdentityUserResponse> GetUser(int tenantId, int id, CancellationToken ct)
    {
        var member = await db.TenantUsers.Include(x => x.User).SingleOrDefaultAsync(x => x.TenantId == tenantId && x.UserId == id, ct)
            ?? throw NotFound("User not found.");
        return await MapUser(member, ct);
    }

    public async Task<UserAccountResponse> CreateUser(CreateUserRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(x => x.Email == email, ct)) throw Conflict("An account with this email already exists.");
        var provider = string.IsNullOrWhiteSpace(request.IdentityProvider) ? "Local" : request.IdentityProvider.Trim();
        ValidateIdentityProvider(provider);
        ValidateGlobalStatus(request.GlobalStatus);
        var user = new User
        {
            Email = email,
            Mobile = request.Mobile?.Trim(),
            IdentityProvider = provider,
            PasswordHash = HashPassword(request.Password),
            GlobalStatus = request.GlobalStatus
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return ToUserAccount(user);
    }

    public async Task<UserAccountResponse> UpdateUser(int id, UpdateUserRequest request, CancellationToken ct)
    {
        var user = await db.Users.SingleOrDefaultAsync(x => x.UserId == id, ct)
            ?? throw NotFound("User not found.");
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(x => x.UserId != id && x.Email == email, ct)) throw Conflict("An account with this email already exists.");
        if (request.IdentityProvider is not null) ValidateIdentityProvider(request.IdentityProvider);
        user.Email = email;
        user.Mobile = request.Mobile?.Trim();
        if (request.IdentityProvider is not null) user.IdentityProvider = request.IdentityProvider.Trim();
        await db.SaveChangesAsync(ct);
        return ToUserAccount(user);
    }

    public async Task UpdateUserStatus(int id, UpdateUserStatusRequest request, CancellationToken ct)
    {
        var status = request.Status;
        ValidateGlobalStatus(status);
        var user = await db.Users.SingleOrDefaultAsync(x => x.UserId == id, ct) ?? throw NotFound("User not found.");
        user.GlobalStatus = status;
        if (status != "Active")
        {
            var sessions = await db.UserSessions.Where(x => x.UserId == id && x.RevokedAt == null).ToListAsync(ct);
            foreach (var session in sessions) session.RevokedAt = DateTime.UtcNow;
        }
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteUser(int id, CancellationToken ct) => await UpdateUserStatus(id, new UpdateUserStatusRequest("Inactive"), ct);

    public async Task<IReadOnlyList<RoleResponse>> GetUserRoles(int id, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        return await db.TenantUserRoles.Where(x => x.TenantUser.UserId == id && (x.EffectiveFrom == null || x.EffectiveFrom <= now) && (x.EffectiveTo == null || x.EffectiveTo > now))
            .Select(x => x.Role).Distinct().OrderBy(x => x.RoleName).Select(x => new RoleResponse(x.RoleId, x.RoleCode, x.RoleName, x.RoleType, x.IsSystemRole)).ToListAsync(ct);
    }

    public async Task AssignRole(int id, AssignRoleRequest request, CancellationToken ct)
    {
        var tenantId = request.TenantId;
        var member = await EnsureMembership(tenantId, id, ct);
        if (!await db.Roles.AnyAsync(x => x.TenantId == tenantId && x.RoleId == request.RoleId, ct)) throw NotFound("Role not found.");
        if (await db.TenantUserRoles.AnyAsync(x => x.TenantId == tenantId && x.TenantUserId == member.TenantUserId && x.RoleId == request.RoleId && (x.EffectiveTo == null || x.EffectiveTo > DateTime.UtcNow), ct)) throw Conflict("The role is already assigned.");
        db.TenantUserRoles.Add(new TenantUserRole { TenantId = tenantId, TenantUserId = member.TenantUserId, RoleId = request.RoleId, OrganizationId = request.OrganizationId, ProjectId = request.ProjectId, EffectiveFrom = request.EffectiveFrom ?? DateTime.UtcNow, EffectiveTo = request.EffectiveTo });
        await db.SaveChangesAsync(ct);
    }

    public async Task RemoveRole(int id, int roleId, int tenantId, CancellationToken ct)
    {
        var assignment = await db.TenantUserRoles.Include(x => x.Role).Include(x => x.TenantUser).FirstOrDefaultAsync(x => x.TenantId == tenantId && x.TenantUser.UserId == id && x.RoleId == roleId && x.EffectiveTo == null, ct)
            ?? throw NotFound("Role assignment not found.");
        if (assignment.Role.RoleCode == "Admin") await EnsureNotLastAdmin(tenantId, id, ct);
        assignment.EffectiveTo = DateTime.UtcNow; await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<RoleResponse>> GetRoles(CancellationToken ct)
    {
        return await db.Roles.OrderBy(x => x.RoleName).Select(RoleProjection()).ToListAsync(ct);
    }

    public async Task<RoleResponse> GetRole(int id, CancellationToken ct) => await db.Roles.Where(x => x.RoleId == id).Select(RoleProjection()).SingleOrDefaultAsync(ct) ?? throw NotFound("Role not found.");

    public async Task<RoleResponse> CreateRole(CreateRoleRequest request, CancellationToken ct)
    {
        var tenantId = request.TenantId;
        await EnsureTenant(tenantId, ct);
        if (await db.Roles.AnyAsync(x => x.TenantId == tenantId && x.RoleCode == request.RoleCode, ct)) throw Conflict("Role code already exists.");
        var role = new Role { TenantId = tenantId, RoleCode = request.RoleCode.Trim(), RoleName = request.RoleName.Trim(), RoleType = "Custom", IsSystemRole = false };
        db.Roles.Add(role); await db.SaveChangesAsync(ct); return Map(role);
    }

    public async Task<RoleResponse> UpdateRole(int id, UpdateRoleRequest request, CancellationToken ct)
    {
        var tenantId = request.TenantId;
        var role = await db.Roles.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.RoleId == id, ct) ?? throw NotFound("Role not found.");
        if (role.IsSystemRole) throw Forbidden("System roles cannot be changed.");
        role.RoleName = request.RoleName.Trim(); await db.SaveChangesAsync(ct); return Map(role);
    }

    public async Task DeleteRole(int id, int tenantId, CancellationToken ct)
    {
        var role = await db.Roles.Include(x => x.RolePermissions).Include(x => x.TenantUserRoles).SingleOrDefaultAsync(x => x.TenantId == tenantId && x.RoleId == id, ct) ?? throw NotFound("Role not found.");
        if (role.IsSystemRole) throw Forbidden("System roles cannot be deleted.");
        if (role.RolePermissions.Count > 0 || role.TenantUserRoles.Count > 0) throw Conflict("Role is assigned or has permissions and cannot be deleted.");
        db.Roles.Remove(role); await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<RolePermissionResponse>> GetRolePermissions(int id, CancellationToken ct)
    {
        await GetRole(id, ct);
        return await db.RolePermissions.Where(x => x.RoleId == id).OrderBy(x => x.Permission.PermissionCode)
            .Select(x => new RolePermissionResponse(x.PermissionId, x.Permission.PermissionCode, x.AccessScope)).ToListAsync(ct);
    }

    public async Task SetRolePermissions(int id, UpdateRolePermissionsRequest request, CancellationToken ct)
    {
        var tenantId = request.TenantId;
        var role = await db.Roles.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.RoleId == id, ct) ?? throw NotFound("Role not found.");
        if (role.IsSystemRole) throw Forbidden("System role permissions cannot be changed.");
        var requested = request.Permissions ?? [];
        if (requested.Select(x => x.PermissionId).Distinct().Count() != requested.Count) throw BadRequest("Permission IDs must be unique.");
        if (requested.Any(x => x.AccessScope is not ("Global" or "Organization" or "Branch" or "Department" or "Project" or "Own"))) throw BadRequest("AccessScope must be Global, Organization, Branch, Department, Project, or Own.");
        var ids = requested.Select(x => x.PermissionId).ToArray();
        if (await db.Permissions.CountAsync(x => ids.Contains(x.PermissionId), ct) != ids.Length) throw BadRequest("One or more permissions do not exist.");
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        db.RolePermissions.RemoveRange(await db.RolePermissions.Where(x => x.TenantId == tenantId && x.RoleId == id).ToListAsync(ct));
        db.RolePermissions.AddRange(requested.Select(x => new RolePermission { TenantId = tenantId, RoleId = id, PermissionId = x.PermissionId, AccessScope = x.AccessScope }));
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct);
    }

    public async Task<IReadOnlyList<PermissionResponse>> GetPermissions(CancellationToken ct) => await db.Permissions.OrderBy(x => x.ModuleCode).ThenBy(x => x.ActionCode).Select(PermissionProjection()).ToListAsync(ct);
    public async Task<PermissionResponse> GetPermission(int id, CancellationToken ct) => await db.Permissions.Where(x => x.PermissionId == id).Select(PermissionProjection()).SingleOrDefaultAsync(ct) ?? throw NotFound("Permission not found.");

    public async Task<PermissionResponse> CreatePermission(CreatePermissionRequest request, CancellationToken ct)
    {
        var permission = new Permission { PermissionCode = request.PermissionCode.Trim(), ModuleCode = request.ModuleCode.Trim(), ActionCode = request.ActionCode.Trim(), Description = request.Description };
        db.Permissions.Add(permission);
        try { await db.SaveChangesAsync(ct); } catch (DbUpdateException) { throw Conflict("Permission code already exists."); }
        return Map(permission);
    }

    public async Task<PermissionResponse> UpdatePermission(int id, UpdatePermissionRequest request, CancellationToken ct)
    {
        var permission = await db.Permissions.FindAsync([id], ct) ?? throw NotFound("Permission not found.");
        permission.ModuleCode = request.ModuleCode.Trim(); permission.ActionCode = request.ActionCode.Trim(); permission.Description = request.Description;
        await db.SaveChangesAsync(ct); return Map(permission);
    }

    public async Task DeletePermission(int id, CancellationToken ct)
    {
        var permission = await db.Permissions.Include(x => x.RolePermissions).SingleOrDefaultAsync(x => x.PermissionId == id, ct) ?? throw NotFound("Permission not found.");
        if (permission.RolePermissions.Count > 0) throw Conflict("Permission is assigned to one or more roles and cannot be deleted.");
        db.Permissions.Remove(permission); await db.SaveChangesAsync(ct);
    }

    private async Task<AuthResponse> CreateSession(User user, Tenant tenant, TenantUser member, string? deviceId, CancellationToken ct)
    {
        var mappedUser = await MapUser(member, ct);
        var access = jwtTokenService.CreateAccessToken(user, member, mappedUser.Roles);
        var refresh = NewToken();
        var session = new UserSession { UserId = user.UserId, TenantId = tenant.TenantId, SessionTokenHash = TokenHash(access.Token), RefreshTokenHash = TokenHash(refresh), DeviceId = deviceId, ExpiresAt = access.ExpiresAt, RefreshExpiresAt = DateTime.UtcNow.AddDays(30) };
        db.UserSessions.Add(session); await db.SaveChangesAsync(ct);
        return new AuthResponse(access.Token, session.ExpiresAt, refresh, session.RefreshExpiresAt, mappedUser);
    }

    private async Task AssignDefaultEmployeeRole(int tenantId, int tenantUserId, CancellationToken ct)
    {
        var roleId = await db.Roles.Where(x => x.TenantId == tenantId && x.RoleCode == "Employee").Select(x => (int?)x.RoleId).SingleOrDefaultAsync(ct);
        if (roleId.HasValue) db.TenantUserRoles.Add(new TenantUserRole { TenantId = tenantId, TenantUserId = tenantUserId, RoleId = roleId.Value, EffectiveFrom = DateTime.UtcNow });
    }

    private static void ValidateIdentityProvider(string provider)
    {
        if (provider is not ("Local" or "Google" or "Microsoft" or "AzureAD" or "Apple" or "Other"))
            throw BadRequest("IdentityProvider must be Local, Google, Microsoft, AzureAD, Apple, or Other.");
    }

    private static void ValidateGlobalStatus(string status)
    {
        if (status is not ("Active" or "Inactive" or "Locked" or "Suspended"))
            throw BadRequest("GlobalStatus must be Active, Inactive, Locked, or Suspended.");
    }

    private static UserAccountResponse ToUserAccount(User user) =>
        new(user.UserId, user.Email, user.Mobile, user.IdentityProvider, user.GlobalStatus, user.LastLoginAt);

    private async Task<IdentityUserResponse> MapUser(TenantUser member, CancellationToken ct)
    {
        var roles = await db.TenantUserRoles.Where(x => x.TenantId == member.TenantId && x.TenantUserId == member.TenantUserId && (x.EffectiveFrom == null || x.EffectiveFrom <= DateTime.UtcNow) && (x.EffectiveTo == null || x.EffectiveTo > DateTime.UtcNow)).Select(x => x.Role.RoleCode).ToListAsync(ct);
        return new IdentityUserResponse(member.UserId, member.TenantUserId, member.User.Email, member.User.Mobile, member.DisplayName, member.User.GlobalStatus, member.Status, roles);
    }

    private async Task<Tenant> EnsureTenant(int tenantId, CancellationToken ct) => await db.Tenants.FindAsync([tenantId], ct) ?? throw NotFound("Tenant not found.");
    private async Task<TenantUser> EnsureMembership(int tenantId, int userId, CancellationToken ct) => await db.TenantUsers.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.UserId == userId, ct) ?? throw NotFound("Tenant membership not found.");

    private async Task EnsureNotLastAdmin(int tenantId, int removingUserId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var admins = await db.TenantUserRoles.CountAsync(x => x.TenantId == tenantId && x.Role.RoleCode == "Admin" && x.TenantUser.Status == "Active" && x.TenantUser.User.GlobalStatus == "Active" && (x.EffectiveFrom == null || x.EffectiveFrom <= now) && (x.EffectiveTo == null || x.EffectiveTo > now), ct);
        if (admins <= 1) throw Conflict("The last active Admin cannot be removed.");
    }

    private static System.Linq.Expressions.Expression<Func<Role, RoleResponse>> RoleProjection() => x => new RoleResponse(x.RoleId, x.RoleCode, x.RoleName, x.RoleType, x.IsSystemRole);
    private static System.Linq.Expressions.Expression<Func<Permission, PermissionResponse>> PermissionProjection() => x => new PermissionResponse(x.PermissionId, x.PermissionCode, x.ModuleCode, x.ActionCode, x.Description);
    private static RoleResponse Map(Role x) => new(x.RoleId, x.RoleCode, x.RoleName, x.RoleType, x.IsSystemRole);
    private static PermissionResponse Map(Permission x) => new(x.PermissionId, x.PermissionCode, x.ModuleCode, x.ActionCode, x.Description);
    private static string NewToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    public static string TokenHash(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    private static string HashPassword(string password) { var salt = RandomNumberGenerator.GetBytes(16); var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 210_000, HashAlgorithmName.SHA256, 32); return $"PBKDF2-SHA256$210000${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}"; }
    private static bool VerifyPassword(string password, string stored)
    {
        var parts = stored.Split('$');
        if (parts.Length != 4 || parts[0] != "PBKDF2-SHA256" || !int.TryParse(parts[1], out var iterations)) return false;
        try { var salt = Convert.FromBase64String(parts[2]); var expected = Convert.FromBase64String(parts[3]); var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length); return CryptographicOperations.FixedTimeEquals(actual, expected); }
        catch (FormatException) { return false; }
    }
    private static IdentityApiException BadRequest(string m) => new(400, "Bad Request", m, 40012);
    private static IdentityApiException Unauthorized(string m) => new(401, "Unauthorized", m, 40101);
    private static IdentityApiException Forbidden(string m) => new(403, "Forbidden", m, 40301);
    private static IdentityApiException NotFound(string m) => new(404, "Not Found", m, 40404);
    private static IdentityApiException Conflict(string m) => new(409, "Conflict", m, 40901);
}
