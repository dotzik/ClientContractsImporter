using System.Xml;
using System.Xml.Serialization;
using ClientContractsImporter.Contracts.Enums;
using ClientContractsImporter.Contracts.Exporters;
using ClientContractsImporter.Contracts.Models;

namespace ClientContractsImporter.Core.Exporters;

public class XmlClientExporter : IClientExporter
{
    public ExportFormat Format => ExportFormat.Xml;

    public async Task<byte[]> ExportAsync(
        IEnumerable<Client> clients,
        CancellationToken cancellationToken = default)
    {
        var clientsList = clients.ToList();

        using var memoryStream = new MemoryStream();

        // Použití XmlWriter s asynchronními operacemi
        var settings = new XmlWriterSettings
        {
            Async = true, // Důležité - povolí asynchronní operace
            Indent = true
        };

        using (var xmlWriter = XmlWriter.Create(memoryStream, settings))
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var serializer = new XmlSerializer(typeof(List<Client>));

            // Začít serializaci
            await xmlWriter.WriteStartDocumentAsync();

            // Provést serializaci (bohužel XmlSerializer nemá přímo async metodu)
            // Ale můžeme asynchronně zapsat do xmlWriter předtím a potom
            serializer.Serialize(xmlWriter, clientsList, namespaces);

            await xmlWriter.FlushAsync();
        }

        return memoryStream.ToArray();
    }
}
