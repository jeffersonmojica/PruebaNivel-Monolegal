using FacturaFlow.Api.Models;
using FacturaFlow.Api.Repositories;

namespace FacturaFlow.Api.Services;
public sealed class ReminderService(IFacturaRepository repository, IEmailSender emailSender, ILogger<ReminderService> logger) : IReminderService
{
    public Task<IReadOnlyList<Factura>> GetAllAsync(CancellationToken ct) => repository.GetAllAsync(ct);
    public async Task<ProcesamientoResultado> ProcessAsync(string id, CancellationToken ct)
    {
        var factura = await repository.GetByIdAsync(id, ct);
        if (factura is null) return new(id, "", "", false, "No se encontró la factura.", null);
        var (next, subject, body) = factura.Estado switch
        {
            "primerrecordatorio" => ("segundorecordatorio", $"Segundo recordatorio: factura {factura.Numero}", $"Hola {factura.ClienteNombre}, la factura {factura.Numero} por {factura.Valor:C} continúa pendiente. Este correo corresponde al segundo recordatorio."),
            "segundorecordatorio" => ("desactivado", $"Aviso de desactivación: factura {factura.Numero}", $"Hola {factura.ClienteNombre}, la factura {factura.Numero} continúa pendiente. El servicio será desactivado conforme a las condiciones aplicables."),
            _ => ("", "", "")
        };
        if (next.Length == 0) return new(id, factura.Numero, factura.ClienteNombre, false, "La factura no tiene un estado procesable.", factura.Estado);
        if (string.IsNullOrWhiteSpace(factura.EmailCliente)) return new(id, factura.Numero, factura.ClienteNombre, false, "El cliente no tiene un correo configurado.", factura.Estado);
        try { await emailSender.SendAsync(factura.EmailCliente, subject, body, ct); }
        catch (Exception ex) when (ex is not OperationCanceledException) { logger.LogError(ex, "Falló el envío para la factura {FacturaId}", id); return new(id, factura.Numero, factura.ClienteNombre, false, "Falló el envío; el estado no fue modificado.", factura.Estado); }
        var now = DateTime.UtcNow;
        var updated = await repository.UpdateAfterEmailAsync(factura, factura.Estado, next, now, ct);
        if (!updated) return new(id, factura.Numero, factura.ClienteNombre, false, "El correo se envió, pero la factura cambió simultáneamente. Revise el registro.", factura.Estado);
        return new(id, factura.Numero, factura.ClienteNombre, true, "Correo enviado y estado actualizado.", next);
    }
    public async Task<IReadOnlyList<ProcesamientoResultado>> ProcessPendingAsync(CancellationToken ct)
    {
        var all = await repository.GetAllAsync(ct);
        var pending = all.Where(x => x.Estado is "primerrecordatorio" or "segundorecordatorio").ToList();
        var results = new List<ProcesamientoResultado>();
        foreach (var invoice in pending) { ct.ThrowIfCancellationRequested(); results.Add(await ProcessAsync(invoice.Id!, ct)); }
        return results;
    }
}
