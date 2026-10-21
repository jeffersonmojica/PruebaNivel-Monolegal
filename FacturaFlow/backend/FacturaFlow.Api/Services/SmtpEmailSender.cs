using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using FacturaFlow.Api.Options;

namespace FacturaFlow.Api.Services;
public sealed class SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly EmailOptions _options = options.Value;
    public async Task SendAsync(string to, string subject, string body, CancellationToken ct)
    {
        if (_options.DryRun) { logger.LogInformation("[DRY-RUN] Correo simulado para {To}. Asunto: {Subject}. Contenido: {Body}", to, subject, body); return; }
        using var message = new MailMessage(_options.From, to, subject, body) { IsBodyHtml = false };
        using var client = new SmtpClient(_options.Host, _options.Port) { EnableSsl = _options.EnableSsl };
        if (!string.IsNullOrWhiteSpace(_options.Username)) client.Credentials = new NetworkCredential(_options.Username, _options.Password);
        ct.ThrowIfCancellationRequested();
        await client.SendMailAsync(message, ct);
    }
}
