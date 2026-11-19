using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using ProjectName.Shared.Configuration;
using Microsoft.Extensions.Logging;

namespace ProjectName.Shared.Services;

/// <summary>
/// Sends email messages using Amazon Simple Email Service (SES).
/// </summary>
/// <remarks>Uses the configured sender address and logs delivery results.</remarks>
public class SesEmailService : IEmailService
{
    private readonly IAmazonSimpleEmailService _ses;
    private readonly ProjectSettings _settings;
    private readonly ILogger<SesEmailService> _logger; 

    /// <summary>
    /// Initializes a new instance of the SesEmailService class.
    /// </summary>
    /// <param name="ses">The Amazon Simple Email Service client.</param>
    /// <param name="settings">The project settings used to configure email delivery.</param>
    /// <param name="logger">The logger used to record email service operations.</param>
    public SesEmailService(IAmazonSimpleEmailService ses, ProjectSettings settings, ILogger<SesEmailService> logger)
    {
        _ses = ses;
        _settings = settings;
        _logger = logger;
    }

    /// <summary>
	/// Sends an email using Amazon SES with the provided template content and receiver's email address.
	/// </summary>
	/// <param name="message">The template content of the email.</param>
	/// <param name="receiver">The email address of the receiver.</param>
	/// <returns>True if the email was sent successfully, otherwise false.</returns>
    public async Task<bool> SendAsync(EmailContent message)
    {
        _logger.LogInformation("Sending email to {Receiver}", message.Receiver);
        try
        {
            var fromAddress = _settings.Email.FromAddress;

            var sendRequest = new SendEmailRequest
            {
                Source = fromAddress,
                Destination = new Destination
                {
                    ToAddresses = new List<string> { message.Receiver }
                },
                Message = new Message
                {
                    Subject = new Content(message.Subject),
                    Body = new Body
                    {
                        Html = new Content(message.Body),
                        Text = string.IsNullOrWhiteSpace(message.Body)
                            ? null
                            : new Content(message.Body)
                    }
                }
            };

            await _ses.SendEmailAsync(sendRequest, CancellationToken.None);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Receiver}: {Message}", message.Receiver, ex.Message);
            return false;
        }
    }
}