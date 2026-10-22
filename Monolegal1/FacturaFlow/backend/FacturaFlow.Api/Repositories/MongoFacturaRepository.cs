using FacturaFlow.Api.Models;
using FacturaFlow.Api.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FacturaFlow.Api.Repositories;
public sealed class MongoFacturaRepository : IFacturaRepository
{
    private readonly IMongoCollection<Factura> _collection;
    public MongoFacturaRepository(IMongoClient client, IOptions<MongoOptions> options)
    {
        var value = options.Value;
        _collection = client.GetDatabase(value.Database).GetCollection<Factura>(value.Collection);
    }
    public async Task<IReadOnlyList<Factura>> GetAllAsync(CancellationToken ct) => await _collection.Find(FilterDefinition<Factura>.Empty).SortByDescending(x => x.FechaVencimiento).ToListAsync(ct);
    public Task<Factura?> GetByIdAsync(string id, CancellationToken ct) => _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);
    public async Task<bool> UpdateAfterEmailAsync(Factura factura, string estadoAnterior, string estadoNuevo, DateTime fecha, CancellationToken ct)
    {
        var filter = Builders<Factura>.Filter.And(Builders<Factura>.Filter.Eq(x => x.Id, factura.Id), Builders<Factura>.Filter.Eq(x => x.Estado, estadoAnterior));
        var update = Builders<Factura>.Update.Set(x => x.Estado, estadoNuevo).Set(x => x.UltimoRecordatorio, fecha)
            .Push(x => x.HistorialEnvios, new RegistroEnvio { EstadoAnterior = estadoAnterior, EstadoNuevo = estadoNuevo, Fecha = fecha, Destinatario = factura.EmailCliente });
        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);
        return result.ModifiedCount == 1;
    }
}
