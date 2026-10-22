using FacturaFlow.Api.Models;
using FacturaFlow.Api.Repositories;
using FacturaFlow.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FacturaFlow.Tests;
public sealed class ReminderServiceTests
{
    [Theory]
    [InlineData("primerrecordatorio", "segundorecordatorio")]
    [InlineData("segundorecordatorio", "desactivado")]
    public async Task SendsEmailThenUpdatesState(string initial, string expected)
    {
        var invoice = new Factura { Id = "abc", Numero = "F-001", ClienteNombre = "Cliente Demo", EmailCliente = "demo@example.com", Estado = initial, Valor = 50000 };
        var repo = new FakeRepository(invoice); var email = new FakeEmailSender();
        var service = new ReminderService(repo, email, NullLogger<ReminderService>.Instance);
        var result = await service.ProcessAsync("abc", CancellationToken.None);
        Assert.True(result.Exito); Assert.Equal(expected, result.EstadoNuevo); Assert.Equal(expected, repo.Invoice!.Estado); Assert.Equal(1, email.Calls);
    }
    [Fact] public async Task DoesNotUpdateStateWhenEmailFails()
    {
        var invoice = new Factura { Id = "abc", Numero = "F-001", ClienteNombre = "Cliente Demo", EmailCliente = "demo@example.com", Estado = "primerrecordatorio" };
        var repo = new FakeRepository(invoice); var email = new FakeEmailSender { ShouldFail = true };
        var result = await new ReminderService(repo, email, NullLogger<ReminderService>.Instance).ProcessAsync("abc", CancellationToken.None);
        Assert.False(result.Exito); Assert.Equal("primerrecordatorio", repo.Invoice!.Estado); Assert.Equal(0, repo.Updates);
    }
    [Fact] public async Task DoesNotSendForDisabledInvoice()
    {
        var repo = new FakeRepository(new Factura { Id="abc", Numero="F-1", Estado="desactivado" }); var email = new FakeEmailSender();
        var result = await new ReminderService(repo, email, NullLogger<ReminderService>.Instance).ProcessAsync("abc", CancellationToken.None);
        Assert.False(result.Exito); Assert.Equal(0, email.Calls); Assert.Equal(0, repo.Updates);
    }
    private sealed class FakeRepository(Factura invoice) : IFacturaRepository
    {
        public Factura? Invoice { get; private set; } = invoice; public int Updates { get; private set; }
        public Task<IReadOnlyList<Factura>> GetAllAsync(CancellationToken ct) => Task.FromResult<IReadOnlyList<Factura>>(new[] { Invoice! });
        public Task<Factura?> GetByIdAsync(string id, CancellationToken ct) => Task.FromResult(Invoice?.Id == id ? Invoice : null);
        public Task<bool> UpdateAfterEmailAsync(Factura f,string old,string next,DateTime date,CancellationToken ct){Updates++; if(Invoice?.Estado!=old)return Task.FromResult(false);Invoice.Estado=next;return Task.FromResult(true);}
    }
    private sealed class FakeEmailSender : IEmailSender { public int Calls {get;private set;} public bool ShouldFail {get;set;} public Task SendAsync(string to,string subject,string body,CancellationToken ct){Calls++;return ShouldFail?Task.FromException(new InvalidOperationException("SMTP error")):Task.CompletedTask;} }
}
