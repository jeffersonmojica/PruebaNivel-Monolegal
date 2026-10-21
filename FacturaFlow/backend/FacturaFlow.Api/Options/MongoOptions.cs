namespace FacturaFlow.Api.Options;
public sealed class MongoOptions
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string Database { get; set; } = "facturaflow";
    public string Collection { get; set; } = "facturas";
}
