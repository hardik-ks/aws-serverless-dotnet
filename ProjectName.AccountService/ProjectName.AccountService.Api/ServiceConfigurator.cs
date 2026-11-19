using ProjectName.Shared;
using ProjectName.AccountService.Application.Interfaces;
using ProjectName.AccountService.Repository.Implementations;
using ProjectName.AccountService.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectName.AccountService.Api;

public static class ServiceConfigurator
{
    public static ServiceProvider Configure()
    {
        var services = new ServiceCollection();

        var configuration = ProjectName.Shared.ConfigurationProvider.BuildConfiguration();

        // Shared infrastructure
        services.AddSharedInfrastructure(configuration);

        // Repositories
        services.AddScoped<IAccountRepository, AccountRepository>();

        // Services
        services.AddScoped<IAccountService, Application.Services.AccountService>();

        return services.BuildServiceProvider();
    }
}
