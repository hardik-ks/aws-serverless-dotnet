using ProjectName.Shared.Contracts;
using System.Text.Json;

namespace ProjectName.Shared.Helpers;

/// <summary>
/// Provides methods for deserializing device requests and populating device information from request headers.
/// </summary>
public static class DeviceRequestHelper
{
    /// <summary>
    /// Provides JSON serializer options with case-insensitive property name matching.
    /// </summary>
    private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

    /// <summary>
    /// Deserializes a request body and applies device information from the supplied headers.
    /// </summary>
    /// <typeparam name="T">The request type that implements <see cref="IDeviceInfo"/>.</typeparam>
    /// <param name="body">The serialized request body.</param>
    /// <param name="headers">The headers containing device information.</param>
    /// <returns>The deserialized request populated with device information.</returns>
    public static T GetRequest<T>(string body, IDictionary<string, string> headers) where T : class, IDeviceInfo
    {
        var dto = Deserialize<T>(body);

        SetDeviceInfo(dto, headers);

        return dto;
    }

    /// <summary>
    /// Deserializes a JSON string into an instance of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object represented by the JSON string.</typeparam>
    /// <param name="body">The JSON string to deserialize.</param>
    /// <returns>The deserialized instance of type T.</returns>
    public static T Deserialize<T>(string body)
    {
        return JsonSerializer.Deserialize<T>(body, _jsonOptions)!;
    }

    /// <summary>
    /// Sets the device ID, name, and platform from the request headers.
    /// </summary>
    /// <param name="dto">The device information to populate.</param>
    /// <param name="headers">The request headers containing the device information.</param>
    private static void SetDeviceInfo(IDeviceInfo dto, IDictionary<string, string> headers)
    {
        dto.DeviceId = headers.TryGetValue("x-device-id", out var deviceId)
            ? deviceId
            : string.Empty;

        dto.DeviceName = headers.TryGetValue("x-device-name", out var deviceName)
            ? deviceName
            : string.Empty;

        dto.DevicePlatform = headers.TryGetValue("x-device-platform", out var devicePlatform)
            ? devicePlatform
            : string.Empty;
    }
}