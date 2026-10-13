using BalkalaInfra.Application.Interfaces;
using BalkalaInfra.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BalkalaInfra.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IClientInquiryService, ClientInquiryService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IDashboardDefinitionService, DashboardDefinitionService>();
        services.AddScoped<ILabourDeploymentService, LabourDeploymentService>();
        return services;
    }
}
