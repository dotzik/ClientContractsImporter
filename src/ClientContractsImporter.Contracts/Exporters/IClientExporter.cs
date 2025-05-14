using ClientContractsImporter.Contracts.Enums;
using ClientContractsImporter.Contracts.Models;

namespace ClientContractsImporter.Contracts.Exporters;

public interface IClientExporter
{
    ExportFormat Format { get; }

    Task<byte[]> ExportAsync(
        IEnumerable<Client> clients,
        CancellationToken cancellationToken = default);
}
