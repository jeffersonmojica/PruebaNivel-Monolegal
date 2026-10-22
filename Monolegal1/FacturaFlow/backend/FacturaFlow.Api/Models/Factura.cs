
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FacturaFlow.Api.Models;

public sealed class Factura
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("clienteId")]
    public string ClienteId { get; set; } = "";

    [BsonElement("clienteNombre")]
    public string ClienteNombre { get; set; } = "";

    [BsonElement("emailCliente")]
    public string EmailCliente { get; set; } = "";

    [BsonElement("numero")]
    public string Numero { get; set; } = "";

    [BsonElement("valor")]
    public decimal Valor { get; set; }

    [BsonElement("fechaVencimiento")]
    public DateTime FechaVencimiento { get; set; }

    [BsonElement("estado")]
    public string Estado { get; set; } = "primerrecordatorio";

    [BsonElement("ultimoRecordatorio")]
    public DateTime? UltimoRecordatorio { get; set; }

    [BsonElement("historialEnvios")]
    public List<RegistroEnvio> HistorialEnvios { get; set; } = [];
}

public sealed class RegistroEnvio
{
    [BsonElement("estadoAnterior")]
    public string EstadoAnterior { get; set; } = "";

    [BsonElement("estadoNuevo")]
    public string EstadoNuevo { get; set; } = "";

    [BsonElement("fecha")]
    public DateTime Fecha { get; set; }

    [BsonElement("destinatario")]
    public string Destinatario { get; set; } = "";
}

public sealed record ProcesamientoResultado(
    string FacturaId,
    string Numero,
    string Cliente,
    bool Exito,
    string Mensaje,
    string? EstadoNuevo);
