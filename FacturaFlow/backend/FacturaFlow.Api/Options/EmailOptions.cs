namespace FacturaFlow.Api.Options;
public sealed class EmailOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 1025;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string From { get; set; } = "recordatorios@example.com";
    public bool EnableSsl { get; set; } = false;
    public bool DryRun { get; set; } = true;
}
