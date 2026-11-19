using ProjectName.Shared.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace ProjectName.Shared.Services;

/// <summary>
/// Email implementation of ICommunicationService using SMTP
/// </summary>
public class EmailService : IEmailService
{
    private readonly ProjectSettings _settings;
    private readonly ILogger<EmailService> _logger;

    /// <summary>
    /// Initializes a new instance of the EmailService class.
    /// </summary>
    /// <param name="settings">The project settings used by the email service.</param>
    /// <param name="logger">The logger used to record email service activity.</param>
    /// <exception cref="ArgumentNullException">Thrown when settings or logger is null.</exception>
    public EmailService(ProjectSettings settings, ILogger<EmailService> logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Sends an email message to the specified receiver
    /// </summary>
    /// <param name="message">The email message content (supports HTML)</param>
    /// <param name="receiver">The email address to send to (receiver)</param>
    /// <returns>True if email was sent successfully, false otherwise</returns>
    public async Task<bool> SendAsync(EmailContent message)
    {
        try
        {
            _logger.LogInformation("Sending email to {Receiver}", message.Receiver);

            // Validate email settings
            if (_settings.Email == null ||
                string.IsNullOrEmpty(_settings.Email.SmtpHost) ||
                string.IsNullOrEmpty(_settings.Email.FromAddress))
            {
                _logger.LogError("Email settings are not configured properly");
                return false;
            }

            using var smtpClient = new SmtpClient(_settings.Email.SmtpHost, _settings.Email.SmtpPort)
            {
                EnableSsl = _settings.Email.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    _settings.Email.Username,
                    _settings.Email.Password
                )
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.Email.FromAddress, _settings.Email.FromName),
                Subject = string.IsNullOrEmpty(message.Subject) ? (_settings.Email.DefaultSubject ?? "ProjectName Notification") : message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(message.Receiver);

            await smtpClient.SendMailAsync(mailMessage);

            _logger.LogInformation("Email sent successfully to {Receiver}", message.Receiver);
            return true;
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex, "SMTP error while sending email to {Receiver}: {Message}", message.Receiver, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Receiver}: {Message}", message.Receiver, ex.Message);
            return false;
        }
    }
}
