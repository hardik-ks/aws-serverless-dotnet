using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using ProjectName.AccountService.Application.Interfaces;
using ProjectName.Shared.Middleware;
using ProjectName.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ProjectName.AccountService.Api;

public class AccountServiceLambda
{
    private static ServiceProvider? _sharedProvider;
    private static readonly object _lock = new object();

    private readonly ServiceProvider _provider;
    private readonly IAccountService _Service;
    private readonly ILogger<AccountServiceLambda> _logger;
    private readonly ILocalizationService _localizationService;
    private readonly bool _ownsProvider;

    public AccountServiceLambda()
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

        _Service = _provider.GetRequiredService<IAccountService>();
        _logger = _provider.GetRequiredService<ILogger<AccountServiceLambda>>();
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

        var result = await _Service.GetByIdAsync(id);

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
        var result = await _Service.GetAllAsync();

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
