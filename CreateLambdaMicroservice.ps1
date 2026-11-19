# ====================================================
# PowerShell Script: CreateLambdaMicroservice.ps1
# Description: Creates a new Lambda microservice for K-Bills backend
# ====================================================

# ----------------------------
# 1. Prompt user for input
# ----------------------------
$serviceName = Read-Host "Enter the microservice name (e.g., VendorService, BillingService)"
if ([string]::IsNullOrWhiteSpace($serviceName)) { 
    Write-Host "❌ Service name is required!"
    exit 
}

# Ensure service name ends with "Service"
if (-not $serviceName.EndsWith("Service")) {
    $serviceName = "$serviceName" + "Service"
}

$baseName = "ProjectName.$serviceName"
Write-Host "Creating microservice: $baseName"

# Get the solution root (assuming script is run from solution root)
$solutionRoot = Get-Location
$solutionFile = Get-ChildItem -Path $solutionRoot -Filter "*.slnx" | Select-Object -First 1

if (-not $solutionFile) {
    Write-Host "❌ No solution file (.sln) found in current directory!"
    exit
}

$solutionPath = $solutionFile.FullName
Write-Host "Using solution: $solutionPath"

# ----------------------------
# 2. Define project structure
# ----------------------------
$projects = @{
    "Api"           = "classlib"
    "Application"   = "classlib"
    "Domain"        = "classlib"
    "Repository"    = "classlib"
}

$serviceFolder = Join-Path $solutionRoot $baseName
New-Item -ItemType Directory -Path $serviceFolder -Force | Out-Null

# ----------------------------
# 3. Create projects
# ----------------------------
foreach ($projName in $projects.Keys) {
    $projType = $projects[$projName]
    $projFullName = "$baseName.$projName"
    $projPath = Join-Path $serviceFolder $projFullName

    Write-Host "Creating project: $projFullName"
    dotnet new $projType -n $projFullName -o $projPath --framework net10.0

    # Remove default Class1.cs
    $class1 = Join-Path $projPath "Class1.cs"
    if (Test-Path $class1) {
        Remove-Item $class1 -Force
    }

    # Add to solution
    dotnet sln $solutionPath add $projPath
}

Write-Host "✅ Projects created and added to solution."

# ----------------------------
# 4. Create folder structure
# ----------------------------
$folders = @{
    "Api" = @("Handlers", "DTOs")
    "Application" = @("Services", "Interfaces", "DTOs")
    "Domain" = @("Entities", "Enums", "ValueObjects")
    "Repository" = @("Interfaces", "Implementations")
}

foreach ($proj in $folders.Keys) {
    $projPath = Join-Path $serviceFolder "$baseName.$proj"
    foreach ($folder in $folders[$proj]) {
        New-Item -ItemType Directory -Path (Join-Path $projPath $folder) -Force | Out-Null
    }
}

Write-Host "✅ Folder structure created."

# ----------------------------
# 5. Add project references
# ----------------------------
$apiPath = Join-Path $serviceFolder "$baseName.Api"
$applicationPath = Join-Path $serviceFolder "$baseName.Application"
$domainPath = Join-Path $serviceFolder "$baseName.Domain"
$repositoryPath = Join-Path $serviceFolder "$baseName.Repository"
$sharedPath = Join-Path $solutionRoot "ProjectName.Shared"

# Api -> Application
dotnet add $apiPath reference $applicationPath

# Application -> Domain, Repository
dotnet add $applicationPath reference $domainPath
dotnet add $applicationPath reference $repositoryPath

# Repository -> Domain, Shared
dotnet add $repositoryPath reference $domainPath
dotnet add $repositoryPath reference $sharedPath

# Api -> Shared (for ServiceConfigurator)
dotnet add $apiPath reference $sharedPath

Write-Host "✅ Project references added."

# ----------------------------
# 6. Add NuGet packages
# ----------------------------
Write-Host "Adding NuGet packages..."

# Api project packages
dotnet add $apiPath package Amazon.Lambda.APIGatewayEvents --version 3.0.0
dotnet add $apiPath package Amazon.Lambda.Core --version 3.1.0
dotnet add $apiPath package Amazon.Lambda.Serialization.SystemTextJson --version 3.0.0

# Repository project packages
dotnet add $repositoryPath package Dapper --version 2.1.79
dotnet add $repositoryPath package Dapper.Contrib --version 2.0.78
dotnet add $repositoryPath package MySql.Data --version 9.7.0

Write-Host "✅ NuGet packages added."

# ----------------------------
# 7. Create Domain Entity (Example)
# ----------------------------
$entityName = $serviceName.Replace("Service", "")
$entitiesFolder = Join-Path $serviceFolder "$baseName.Domain\Entities"
New-Item -ItemType Directory -Path $entitiesFolder -Force | Out-Null
$entityFile = Join-Path $entitiesFolder "$entityName.cs"

@"
using Dapper.Contrib.Extensions;

namespace $baseName.Domain.Entities;

[Table("${entityName}s")]
public class $entityName
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;
}
"@ | Out-File $entityFile -Encoding UTF8

Write-Host "✅ Domain entity created: $entityName"

# ----------------------------
# 8. Create Repository Interface
# ----------------------------
$repoInterface = Join-Path $serviceFolder "$baseName.Repository\Interfaces\I${entityName}Repository.cs"

@"
using $baseName.Domain.Entities;
using ProjectName.Shared.Repository;

namespace $baseName.Repository.Interfaces;

public interface I${entityName}Repository : IRepositoryBase<$entityName, int>
{
    Task<${entityName}?> GetByIdAsync(int id);
    Task<IEnumerable<$entityName>> GetAllAsync();
    Task<bool> CreateAsync($entityName entity);
    Task<bool> UpdateAsync($entityName entity);
    Task<bool> DeleteAsync(int id);
}
"@ | Out-File $repoInterface -Encoding UTF8

# ----------------------------
# 9. Create Repository Implementation
# ----------------------------
$repoImplFolder = Join-Path $serviceFolder "$baseName.Repository\Implementations"
New-Item -ItemType Directory -Path $repoImplFolder -Force | Out-Null
$repoImpl = Join-Path $repoImplFolder "${entityName}Repository.cs"

@"
using Dapper;
using $baseName.Domain.Entities;
using $baseName.Repository.Interfaces;
using ProjectName.Shared.Repository;

namespace $baseName.Repository.Implementations;

public class ${entityName}Repository : RepositoryBase<$entityName, int>, I${entityName}Repository
{
    public ${entityName}Repository(IConnectionFactory connectionFactory) 
        : base(connectionFactory) 
    { 
    }

    public async Task<${entityName}?> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM ${entityName}s WHERE Id = @Id AND IsActive = 1";

        using var conn = connectionFactory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<$entityName>(sql, new { Id = id });
    }

    public async Task<IEnumerable<$entityName>> GetAllAsync()
    {
        var sql = "SELECT * FROM ${entityName}s WHERE IsActive = 1 ORDER BY CreatedAt DESC";

        using var conn = connectionFactory.CreateConnection();
        return await conn.QueryAsync<$entityName>(sql);
    }

    public async Task<bool> CreateAsync($entityName entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        var id = await InsertAsync(entity);
        return id > 0;
    }

    public async Task<bool> UpdateAsync($entityName entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        return await base.UpdateAsync(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sql = "UPDATE ${entityName}s SET IsActive = 0, UpdatedAt = @UpdatedAt WHERE Id = @Id";

        using var conn = connectionFactory.CreateConnection();
        var rowsAffected = await conn.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
        return rowsAffected > 0;
    }
}
"@ | Out-File $repoImpl -Encoding UTF8

Write-Host "✅ Repository created."

# ----------------------------
# 10. Create Application Service Interface
# ----------------------------
$appInterfacesFolder = Join-Path $serviceFolder "$baseName.Application\Interfaces"
New-Item -ItemType Directory -Path $appInterfacesFolder -Force | Out-Null
$serviceInterface = Join-Path $appInterfacesFolder "I${entityName}Service.cs"

@"
using $baseName.Application.DTOs;

namespace $baseName.Application.Interfaces;

public interface I${entityName}Service
{
    Task<${entityName}Dto?> GetByIdAsync(int id);
    Task<IEnumerable<${entityName}Dto>> GetAllAsync();
    Task<bool> CreateAsync(Create${entityName}Dto dto);
    Task<bool> UpdateAsync(int id, Update${entityName}Dto dto);
    Task<bool> DeleteAsync(int id);
}
"@ | Out-File $serviceInterface -Encoding UTF8

# ----------------------------
# 11. Create DTOs
# ----------------------------
$dtoFile = Join-Path $serviceFolder "$baseName.Application\DTOs\${entityName}Dto.cs"

@"
namespace $baseName.Application.DTOs;

public class ${entityName}Dto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class Create${entityName}Dto
{
    public string Name { get; set; } = string.Empty;
}

public class Update${entityName}Dto
{
    public string Name { get; set; } = string.Empty;
}
"@ | Out-File $dtoFile -Encoding UTF8

# ----------------------------
# 12. Create Application Service
# ----------------------------
$servicesFolder = Join-Path $serviceFolder "$baseName.Application\Services"
New-Item -ItemType Directory -Path $servicesFolder -Force | Out-Null
$serviceImpl = Join-Path $servicesFolder "${entityName}Service.cs"

@"
using $baseName.Application.DTOs;
using $baseName.Application.Interfaces;
using $baseName.Domain.Entities;
using $baseName.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace $baseName.Application.Services;

public class ${entityName}Service : I${entityName}Service
{
    private readonly I${entityName}Repository _repository;
    private readonly ILogger<${entityName}Service> _logger;

    public ${entityName}Service(
        I${entityName}Repository repository,
        ILogger<${entityName}Service> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<${entityName}Dto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return null;

        return MapToDto(entity);
    }

    public async Task<IEnumerable<${entityName}Dto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(MapToDto);
    }

    public async Task<bool> CreateAsync(Create${entityName}Dto dto)
    {
        var entity = new $entityName
        {
            Name = dto.Name
        };

        return await _repository.CreateAsync(entity);
    }

    public async Task<bool> UpdateAsync(int id, Update${entityName}Dto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        entity.Name = dto.Name;

        return await _repository.UpdateAsync(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static ${entityName}Dto MapToDto($entityName entity)
    {
        return new ${entityName}Dto
        {
            Id = entity.Id,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsActive = entity.IsActive
        };
    }
}
"@ | Out-File $serviceImpl -Encoding UTF8

Write-Host "✅ Application service created."

# ----------------------------
# 13. Create ServiceConfigurator
# ----------------------------
$serviceConfigurator = Join-Path $serviceFolder "$baseName.Api\ServiceConfigurator.cs"

@"
using ProjectName.Shared;
using $baseName.Application.Interfaces;
using $baseName.Application.Services;
using $baseName.Repository.Implementations;
using $baseName.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace $baseName.Api;

public class ServiceConfigurator
{
    public static ServiceProvider Configure()
    {
        var services = new ServiceCollection();

        var configuration = ProjectName.Shared.ConfigurationProvider.BuildConfiguration();

        // Shared infrastructure
        services.AddSharedInfrastructure(configuration);

        // Repositories
        services.AddScoped<I${entityName}Repository, ${entityName}Repository>();

        // Services
        services.AddScoped<I${entityName}Service, ${entityName}Service>();

        return services.BuildServiceProvider();
    }
}
"@ | Out-File $serviceConfigurator -Encoding UTF8

# ----------------------------
# 14. Create Lambda Function
# ----------------------------
$lambdaFunction = Join-Path $serviceFolder "$baseName.Api\${serviceName}Lambda.cs"

@"
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using $baseName.Application.Interfaces;
using ProjectName.Shared.Middleware;
using ProjectName.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace $baseName.Api;

public class ${serviceName}Lambda
{
    private static ServiceProvider? _sharedProvider;
    private static readonly object _lock = new object();

    private readonly ServiceProvider _provider;
    private readonly I${entityName}Service _${entityName.ToLower()}Service;
    private readonly ILogger<${serviceName}Lambda> _logger;
    private readonly ILocalizationService _localizationService;
    private readonly bool _ownsProvider;

    public ${serviceName}Lambda()
    {
        // Use environment variable to determine behavior
        var usePerInstanceProvider = Environment.GetEnvironmentVariable("USE_PER_INSTANCE_PROVIDER") == "true";

        if (usePerInstanceProvider)
        {
            _provider = ServiceConfigurator.Configure();
            _ownsProvider = true;
        }
        else
        {
            if (_sharedProvider == null)
            {
                lock (_lock)
                {
                    _sharedProvider ??= ServiceConfigurator.Configure();
                }
            }
            _provider = _sharedProvider;
            _ownsProvider = false;
        }

        _${entityName.ToLower()}Service = _provider.GetRequiredService<I${entityName}Service>();
        _logger = _provider.GetRequiredService<ILogger<${serviceName}Lambda>>();
        _localizationService = _provider.GetRequiredService<ILocalizationService>();
    }

    public async Task<APIGatewayProxyResponse> LambdaHandler(
        APIGatewayProxyRequest request,
        ILambdaContext context)
    {
        try
        {
            // Set language from headers
            var language = LocalizationHelper.GetLanguageFromHeaders(request.Headers);
            LocalizationHelper.SetLanguage(_localizationService, language);

            _logger.LogInformation("Processing {HttpMethod} request to {Path}", 
                request.HttpMethod, request.Path);

            // Route to appropriate handler
            return request.HttpMethod.ToUpper() switch
            {
                "GET" when request.PathParameters?.ContainsKey("id") == true 
                    => await GetByIdAsync(request),
                "GET" => await GetAllAsync(request),
                "POST" => await CreateAsync(request),
                "PUT" => await UpdateAsync(request),
                "DELETE" => await DeleteAsync(request),
                _ => new APIGatewayProxyResponse
                {
                    StatusCode = 405,
                    Body = JsonSerializer.Serialize(new { error = "Method not allowed" })
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request");
            return new APIGatewayProxyResponse
            {
                StatusCode = 500,
                Body = JsonSerializer.Serialize(new { error = "Internal server error" })
            };
        }
    }

    private async Task<APIGatewayProxyResponse> GetByIdAsync(APIGatewayProxyRequest request)
    {
        if (!int.TryParse(request.PathParameters["id"], out var id))
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 400,
                Body = JsonSerializer.Serialize(new { error = "Invalid ID" })
            };
        }

        var result = await _${entityName.ToLower()}Service.GetByIdAsync(id);

        if (result == null)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 404,
                Body = JsonSerializer.Serialize(new { error = "Not found" })
            };
        }

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(result)
        };
    }

    private async Task<APIGatewayProxyResponse> GetAllAsync(APIGatewayProxyRequest request)
    {
        var result = await _${entityName.ToLower()}Service.GetAllAsync();

        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(result)
        };
    }

    private async Task<APIGatewayProxyResponse> CreateAsync(APIGatewayProxyRequest request)
    {
        // Implementation for create
        return new APIGatewayProxyResponse
        {
            StatusCode = 501,
            Body = JsonSerializer.Serialize(new { error = "Not implemented" })
        };
    }

    private async Task<APIGatewayProxyResponse> UpdateAsync(APIGatewayProxyRequest request)
    {
        // Implementation for update
        return new APIGatewayProxyResponse
        {
            StatusCode = 501,
            Body = JsonSerializer.Serialize(new { error = "Not implemented" })
        };
    }

    private async Task<APIGatewayProxyResponse> DeleteAsync(APIGatewayProxyRequest request)
    {
        // Implementation for delete
        return new APIGatewayProxyResponse
        {
            StatusCode = 501,
            Body = JsonSerializer.Serialize(new { error = "Not implemented" })
        };
    }
}
"@ | Out-File $lambdaFunction -Encoding UTF8

Write-Host "✅ Lambda function created."

# ----------------------------
# 15. Update Api.csproj with Lambda settings
# ----------------------------
$apiCsprojPath = Join-Path $serviceFolder "$baseName.Api\$baseName.Api.csproj"
$csprojContent = Get-Content $apiCsprojPath -Raw

# Add Lambda-specific properties
$lambdaProperties = @"
    <GenerateRuntimeConfigurationFiles>true</GenerateRuntimeConfigurationFiles>
    <AWSProjectType>Lambda</AWSProjectType>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
    <PublishReadyToRun>true</PublishReadyToRun>
"@

$csprojContent = $csprojContent -replace '(<TargetFramework>net10.0</TargetFramework>)', "`$1`n$lambdaProperties"
Set-Content -Path $apiCsprojPath -Value $csprojContent

Write-Host "✅ Api project configured for Lambda."

# ----------------------------
# 16. Create Test Projects
# ----------------------------
$testsFolder = Join-Path $solutionRoot "ProjectName.Tests"

# Unit Tests
$unitTestPath = Join-Path $testsFolder "$baseName.Tests"
if (-not (Test-Path $unitTestPath)) {
    dotnet new xunit -n "$baseName.Tests" -o $unitTestPath --framework net10.0
    dotnet sln $solutionPath add $unitTestPath

    # Remove default test file
    $unitTest1 = Join-Path $unitTestPath "UnitTest1.cs"
    if (Test-Path $unitTest1) {
        Remove-Item $unitTest1 -Force
    }

    # Add references
    dotnet add $unitTestPath reference $applicationPath
    dotnet add $unitTestPath reference $repositoryPath

    # Add NuGet packages
    dotnet add $unitTestPath package NSubstitute --version 5.3.0
    dotnet add $unitTestPath package FluentAssertions --version 7.0.0

    Write-Host "✅ Unit test project created."
}

# Integration Tests
$integrationTestPath = Join-Path $testsFolder "$baseName.IntegrationTests"
if (-not (Test-Path $integrationTestPath)) {
    dotnet new xunit -n "$baseName.IntegrationTests" -o $integrationTestPath --framework net10.0
    dotnet sln $solutionPath add $integrationTestPath

    # Remove default test file
    $integrationTest1 = Join-Path $integrationTestPath "UnitTest1.cs"
    if (Test-Path $integrationTest1) {
        Remove-Item $integrationTest1 -Force
    }

    # Add references
    dotnet add $integrationTestPath reference $apiPath
    dotnet add $integrationTestPath reference $applicationPath

    Write-Host "✅ Integration test project created."
}

# ----------------------------
# 17. Create sample unit test
# ----------------------------
New-Item -ItemType Directory -Path $unitTestPath -Force | Out-Null
$serviceTestFile = Join-Path $unitTestPath "${entityName}ServiceTests.cs"

@"
using $baseName.Application.DTOs;
using $baseName.Application.Services;
using $baseName.Domain.Entities;
using $baseName.Repository.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace $baseName.Tests;

public class ${entityName}ServiceTests
{
    private readonly I${entityName}Repository _repository;
    private readonly ILogger<${entityName}Service> _logger;
    private readonly ${entityName}Service _service;

    public ${entityName}ServiceTests()
    {
        _repository = Substitute.For<I${entityName}Repository>();
        _logger = Substitute.For<ILogger<${entityName}Service>>();
        _service = new ${entityName}Service(_repository, _logger);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ReturnsDto()
    {
        // Arrange
        var entity = new $entityName
        {
            Id = 1,
            Name = "Test $entityName",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _repository.GetByIdAsync(1).Returns(entity);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test $entityName");
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityNotFound_ReturnsNull()
    {
        // Arrange
        _repository.GetByIdAsync(999).Returns((($entityName?)null));

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        // Arrange
        var entities = new List<$entityName>
        {
            new $entityName { Id = 1, Name = "Test 1", CreatedAt = DateTime.UtcNow, IsActive = true },
            new $entityName { Id = 2, Name = "Test 2", CreatedAt = DateTime.UtcNow, IsActive = true }
        };
        _repository.GetAllAsync().Returns(entities);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }
}
"@ | Out-File $serviceTestFile -Encoding UTF8

Write-Host "✅ Sample unit tests created."

# ----------------------------
# Final Summary
# ----------------------------
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "✅ Microservice created successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Service Name: $baseName" -ForegroundColor Cyan
Write-Host "Location: $serviceFolder" -ForegroundColor Cyan
Write-Host ""
Write-Host "Projects created:" -ForegroundColor Yellow
Write-Host "  - $baseName.Api (Lambda handlers)" -ForegroundColor White
Write-Host "  - $baseName.Application (Business logic)" -ForegroundColor White
Write-Host "  - $baseName.Domain (Entities)" -ForegroundColor White
Write-Host "  - $baseName.Repository (Data access)" -ForegroundColor White
Write-Host "  - $baseName.Tests (Unit tests)" -ForegroundColor White
Write-Host "  - $baseName.IntegrationTests (Integration tests)" -ForegroundColor White
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Run 'dotnet restore' to restore packages" -ForegroundColor White
Write-Host "  2. Update entity model in Domain/Entities" -ForegroundColor White
Write-Host "  3. Implement business logic in Application/Services" -ForegroundColor White
Write-Host "  4. Create database table for ${entityName}s" -ForegroundColor White
Write-Host "  5. Write unit tests" -ForegroundColor White
Write-Host "  6. Deploy Lambda function to AWS" -ForegroundColor White
Write-Host ""
