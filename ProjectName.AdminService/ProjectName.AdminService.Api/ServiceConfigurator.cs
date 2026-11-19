using ProjectName.Shared;
using ProjectName.AdminService.Application.Interfaces;
using ProjectName.AdminService.Application.Services;
using ProjectName.AdminService.Repository.Implementations;
using ProjectName.AdminService.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectName.AdminService.Api;

public class ServiceConfigurator
{
    public static ServiceProvider Configure()
    {
        var services = new ServiceCollection();

        var configuration = ProjectName.Shared.ConfigurationProvider.BuildConfiguration();

        // Shared infrastructure
        services.AddSharedInfrastructure(configuration);

        // Repositories
        services.AddScoped<IAdminRepository, AdminRepository>();

        // Services
        services.AddScoped<IAdminService, ProjectName.AdminService.Application.Services.AdminService>();

        return services.BuildServiceProvider();
    }
}
