using BalkalaInfra.Application;
using BalkalaInfra.Application.DTOs.Response;
using BalkalaInfra.Infrastructure;
using BalkalaInfra.Infrastructure.Security;
using BalkalaInfra.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Text;
using BalkalaInfra.Infrastructure.Data;
using BalkalaInfra.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// WebApplicationBuilder loads User Secrets automatically in Development. Keep an
// explicit fallback for cases where another configuration provider supplies an
// empty Jwt:SigningKey and prevents the development secret from being selected.
if (builder.Environment.IsDevelopment()
    && string.IsNullOrWhiteSpace(builder.Configuration["Jwt:SigningKey"]))
{
    builder.Configuration.AddUserSecrets(typeof(Program).Assembly, optional: true);
}

// Register services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT access token returned by /api/auth/login."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document, null)] = []
    });
});
builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var firstError = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .Select(entry => new
            {
                Field = ToCamelCase(entry.Key.Split('.').Last()),
                Error = entry.Value!.Errors[0].ErrorMessage
            })
            .FirstOrDefault();

        var message = firstError is null
            ? "The request is invalid."
            : firstError.Error.Contains("required", StringComparison.OrdinalIgnoreCase)
                ? $"The '{firstError.Field}' field is required."
                : firstError.Error;

        var response = new ApiErrorResponse(false, "Bad Request", message, 40012);
        return new BadRequestObjectResult(response);
    };
});

static string ToCamelCase(string value) =>
    value.Length == 0 ? value : char.ToLowerInvariant(value[0]) + value[1..];

var jwtSettings = JwtSettings.FromConfiguration(builder.Configuration);
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var authorization = context.Request.Headers.Authorization.ToString();
                var rawToken = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? authorization[7..].Trim()
                    : string.Empty;
                var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                var tokenHash = IdentityManagementService.TokenHash(rawToken);
                var session = await db.UserSessions.AsNoTracking()
                    .Where(x => x.SessionTokenHash == tokenHash && x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow)
                    .Select(x => new { x.UserId, x.TenantId, x.User.GlobalStatus, TenantStatus = x.Tenant == null ? null : x.Tenant.Status })
                    .SingleOrDefaultAsync(context.HttpContext.RequestAborted);
                if (session is null || session.TenantId is null || session.GlobalStatus != "Active" || session.TenantStatus != "Active")
                {
                    context.Fail("The access token session is revoked, expired, or inactive.");
                    return;
                }

                var tenantId = session.TenantId.Value;
                var isActiveMember = await db.TenantUsers.AsNoTracking().AnyAsync(
                    x => x.UserId == session.UserId && x.TenantId == tenantId && x.Status == "Active",
                    context.HttpContext.RequestAborted);
                if (!isActiveMember)
                {
                    context.Fail("The tenant membership is inactive.");
                    return;
                }

                if (context.Principal?.IsInRole("Admin") == true)
                {
                    var isStillAdmin = await db.TenantUserRoles.AsNoTracking().AnyAsync(x =>
                        x.TenantId == tenantId && x.TenantUser.UserId == session.UserId && x.TenantUser.Status == "Active"
                        && x.Role.RoleCode == "Admin" && (x.EffectiveFrom == null || x.EffectiveFrom <= DateTime.UtcNow)
                        && (x.EffectiveTo == null || x.EffectiveTo > DateTime.UtcNow), context.HttpContext.RequestAborted);
                    if (!isStillAdmin) context.Fail("The Admin role is no longer active.");
                }
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new ApiErrorResponse(false, "Unauthorized", "A valid JWT access token is required.", 40101));
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new ApiErrorResponse(false, "Forbidden", "The authenticated user does not have the required Admin permission.", 40301));
            }
        };
    });
builder.Services.AddAuthorization(options =>
{
    // Admin is the current system-wide administrator role. All controllers are protected
    // by default; only explicitly anonymous endpoints can be reached without an Admin JWT.
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole("Admin")
        .Build();
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStatusCodePages(async statusCodeContext =>
{
    var response = statusCodeContext.HttpContext.Response;
    var statusCode = response.StatusCode;
    var (error, message, code) = statusCode switch
    {
        StatusCodes.Status404NotFound => ("Not Found", "The requested endpoint was not found.", 40404),
        StatusCodes.Status405MethodNotAllowed => ("Method Not Allowed", "The HTTP method is not supported for this endpoint.", 40500),
        _ => ("Request Error", "The request could not be completed.", statusCode * 100)
    };

    response.ContentType = "application/json";
    await response.WriteAsJsonAsync(new ApiErrorResponse(false, error, message, code));
});

// Enable Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.Interceptors.ResponseInterceptorFunction = """
            function (response) {
                var responsePath = response && response.url
                    ? new URL(response.url, window.location.origin).pathname
                    : "";
                var normalizedPath = responsePath.toLowerCase();
                if ((normalizedPath.endsWith("/api/auth/logout") || normalizedPath.endsWith("/api/auth/logout/"))
                    && response.status >= 200 && response.status < 300
                    && window.ui && window.ui.authActions) {
                    window.ui.authActions.logoutWithPersistOption(["Bearer"]);
                }
                return response;
            }
            """;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Root endpoint
app.MapGet("/api/balkalainfra/up", () =>
{
    return "Balkala Infra API is running successfully.";
})
.WithName("GetApiStatus");

app.Run();
