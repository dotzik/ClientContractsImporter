namespace ClientContractsImporter.Contracts.Models;

public class ProductionRecord
{
    public string Period { get; init; }

    public int? Quantity { get; init; }

    // Xml serialization requires a parameterless constructor
    public ProductionRecord()
    {
    }

    public ProductionRecord(string period, int? quantity)
    {
        Period = period;
        Quantity = quantity;
    }
}
