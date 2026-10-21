using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FacturaFlow.Api.Models;
public sealed class Factura
{
    [BsonId] [BsonRepresentation(BsonType.ObjectId)] public string? Id { get; set; }
    public string ClienteId { get; set; } = "";
    public string ClienteNombre { get; set; } = "";
    public string EmailCliente { get; set; } = "";
    public string Numero { get; set; } = "";
    public decimal Valor { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public string Estado { get; set; } = "primerrecordatorio";
    public DateTime? UltimoRecordatorio { get; set; }
    public List<RegistroEnvio> HistorialEnvios { get; set; } = [];
}
public sealed class RegistroEnvio
{
    public string EstadoAnterior { get; set; } = "";
    public string EstadoNuevo { get; set; } = "";
    public DateTime Fecha { get; set; }
    public string Destinatario { get; set; } = "";
}
public sealed record ProcesamientoResultado(string FacturaId, string Numero, string Cliente, bool Exito, string Mensaje, string? EstadoNuevo);
