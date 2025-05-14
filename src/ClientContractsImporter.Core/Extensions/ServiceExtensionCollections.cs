using ClientContractsImporter.Contracts.DataConverters;
using ClientContractsImporter.Contracts.Exporters;
using ClientContractsImporter.Contracts.Handlers;
using ClientContractsImporter.Core.DataConverters;
using ClientContractsImporter.Core.Exporters;
using ClientContractsImporter.Core.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace ClientContractsImporter.Core.Extensions;

public static class ServiceExtensionCollections
{
    public static IServiceCollection AddClientContractsImporter(this IServiceCollection services)
    {
        services.AddSingleton<IClientDataConverter, XlsxClientDataConverter>();
        services.AddSingleton<IClientExporter, JsonClientExporter>();
        services.AddSingleton<IClientExporter, XmlClientExporter>();
        services.AddSingleton<IImportHandler, JsonImportHandler>();

        return services;
    }
}
