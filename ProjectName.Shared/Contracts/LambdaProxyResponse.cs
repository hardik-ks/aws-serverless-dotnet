using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;

namespace ProjectName.Shared.Contracts;

public static class LambdaProxyResponse
{
    /// <summary>
    /// Wraps your ProjectName ApiResponse into API Gateway HTTP API v2 Proxy format
    /// Returns the AWS SDK type that API Gateway expects
    /// </summary>
    public static APIGatewayHttpApiV2ProxyResponse FromApiResponse<T>(
        ApiResponse<T> apiResponse,
        bool enableCors = true)
    {
        var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "X-Trace-Id", apiResponse.TraceId }
            };

        // Optional but recommended for public APIs
        if (enableCors)
        {
            headers["Access-Control-Allow-Origin"] = "*";
            headers["Access-Control-Allow-Headers"] = "Content-Type,Authorization";
            headers["Access-Control-Allow-Methods"] = "GET,POST,PUT,DELETE,OPTIONS";
        }

        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = apiResponse.StatusCode,
            Headers = headers,
            Body = JsonSerializer.Serialize(apiResponse)
        };
    }
}
