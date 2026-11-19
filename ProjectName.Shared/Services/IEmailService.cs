using ProjectName.Shared.Helpers;

namespace ProjectName.Shared.Services;

/// <summary>
/// Interface for Email communication service
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="message">The email content to send.</param>
    /// <returns>A task that represents the asynchronous operation; the task result is <c>true</c> if the email was sent
    /// successfully; otherwise, <c>false</c>.</returns>
    Task<bool> SendAsync(EmailContent message);
}

/// <summary>
/// Represents the content of an email message.
/// </summary>
public class EmailContent
{
    /// <summary>
    /// Gets or sets the subject of the email.
    /// </summary>
    public string Subject { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the body of the email.
    /// </summary>
    public string Body { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the recipient of the email.
    /// </summary>
    public string Receiver { get; set; } = string.Empty;
}