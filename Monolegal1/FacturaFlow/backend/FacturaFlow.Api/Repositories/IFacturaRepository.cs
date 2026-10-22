using FacturaFlow.Api.Models;
namespace FacturaFlow.Api.Repositories;
public interface IFacturaRepository
{
    Task<IReadOnlyList<Factura>> GetAllAsync(CancellationToken ct);
    Task<Factura?> GetByIdAsync(string id, CancellationToken ct);
    Task<bool> UpdateAfterEmailAsync(Factura factura, string estadoAnterior, string estadoNuevo, DateTime fecha, CancellationToken ct);
}
