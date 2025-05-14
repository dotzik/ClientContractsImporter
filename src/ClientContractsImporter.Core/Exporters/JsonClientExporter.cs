using System.Text;
using System.Text.Json;
using ClientContractsImporter.Contracts.Enums;
using ClientContractsImporter.Contracts.Exporters;
using ClientContractsImporter.Contracts.Models;
using Newtonsoft.Json;

namespace ClientContractsImporter.Core.Exporters;

public class JsonClientExporter : IClientExporter
{
    public ExportFormat Format => ExportFormat.Json;

    public async Task<byte[]> ExportAsync(
        IEnumerable<Client> clients,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(clients);

        using var memoryStream = new MemoryStream();

        using (var writer = new Utf8JsonWriter(memoryStream, new JsonWriterOptions { Indented = true }))
        {
            writer.WriteStartArray();

            foreach (var client in clients)
            {
                cancellationToken.ThrowIfCancellationRequested();

                writer.WriteStartObject();

                writer.WriteString("Name", client.Name);
                writer.WriteString("CompanyId", client.CompanyId);

                writer.WriteStartArray("Contracts");
                foreach (var contract in client.Contracts)
                {
                    writer.WriteStartObject();
                    writer.WriteString("Name", contract.Name);

                    writer.WriteStartArray("Records");
                    foreach (var record in contract.Records)
                    {
                        writer.WriteStartObject();
                        writer.WriteString("Period", record.Period);
                        if (record.Quantity.HasValue)
                        {
                            writer.WriteNumber("Quantity", record.Quantity.Value);
                        }
                        else
                        {
                            writer.WriteNull("Quantity");
                        }

                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray();

                    writer.WriteEndObject();
                }

                writer.WriteEndArray();

                writer.WriteEndObject();

                if (memoryStream.Length > 1024 * 1024)
                {
                    await writer.FlushAsync(cancellationToken);
                }
            }

            writer.WriteEndArray();
            await writer.FlushAsync(cancellationToken);
        }

        return memoryStream.ToArray();
    }
}
