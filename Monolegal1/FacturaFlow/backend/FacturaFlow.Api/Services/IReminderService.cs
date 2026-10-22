using FacturaFlow.Api.Models;
namespace FacturaFlow.Api.Services;
public interface IReminderService
{
    Task<IReadOnlyList<Factura>> GetAllAsync(CancellationToken ct);
    Task<ProcesamientoResultado> ProcessAsync(string id, CancellationToken ct);
    Task<IReadOnlyList<ProcesamientoResultado>> ProcessPendingAsync(CancellationToken ct);
}
