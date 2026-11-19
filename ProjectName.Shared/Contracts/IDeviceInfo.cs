namespace ProjectName.Shared.Contracts;

/// <summary>
/// Device Information details
/// </summary>
public interface IDeviceInfo
{
    /// <summary>
    /// Gets or sets the identifier of the device.
    /// </summary>
    string DeviceId { get; set; }

    /// <summary>
    /// Gets or sets the name of the device.
    /// </summary>
    string DeviceName { get; set; }

    /// <summary>
    /// Gets or sets the device platform.
    /// </summary>
    string DevicePlatform { get; set; }
}
