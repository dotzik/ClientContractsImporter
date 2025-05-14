namespace ClientContractsImporter.Contracts.Models;

public class Contract
{
    public string Name { get; init; }

    public List<ProductionRecord> Records { get; set; } = [];

    // Xml serialization requires a parameterless constructor
    public Contract()
    {
    }

    public Contract(string name)
    {
        Name = name;
    }
}
