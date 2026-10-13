using BalkalaInfra.Infrastructure.Data;
using BalkalaInfra.Application.Interfaces;
using BalkalaInfra.Infrastructure.Repositories;
using BalkalaInfra.Infrastructure.Services;
using BalkalaInfra.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BalkalaInfra.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not configured. Set it in " +
                "appsettings.Development.json or the ConnectionStrings__DefaultConnection environment variable.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySQL(connectionString));
        services.AddScoped<IClientInquiryRepository, ClientInquiryRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IIdentityManagementService, IdentityManagementService>();
        services.AddScoped<JwtTokenService>();
        services.AddScoped<IDashboardDefinitionRepository, DashboardDefinitionRepository>();
        services.AddScoped<ILabourDeploymentRepository, LabourDeploymentRepository>();

        return services;
    }
}
