using ClientContractsImporter.Contracts.Models;

namespace ClientContractsImporter.Contracts.DataConverters;

public interface IClientDataConverter
{
    Task<List<Client>> ConvertAsync(
        Stream stream,
        CancellationToken cancellationToken = default);
}
