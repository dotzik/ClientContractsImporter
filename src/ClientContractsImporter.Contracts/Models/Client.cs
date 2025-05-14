using System.Collections.ObjectModel;

namespace ClientContractsImporter.Contracts.Models;

public class Client
{
    public string Name { get; init; }

    public string CompanyId { get; init; }

    public List<Contract> Contracts { get; set; } = [];

    // Xml serialization requires a parameterless constructor
    public Client()
    {
    }

    public Client(string name, string companyId)
    {
        Name = name;
        CompanyId = companyId;
    }
}
