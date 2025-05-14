using ClientContractsImporter.Contracts.Enums;
using ClientContractsImporter.Contracts.Handlers;
using ClientContractsImporter.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddLogging(config => config.AddConsole());
        services.AddClientContractsImporter();
    });

using var host = builder.Build();

var handler = host.Services.GetRequiredService<IImportHandler>();

var input = args.Length > 0 ? args[0] : "test2.xlsx";
var output = args.Length > 1 ? args[1] : "output.json";

if (!Enum.TryParse<ExportFormat>(args.Length > 2 ? args[2] : "json", true, out var format))
{
    Console.WriteLine("Neplatný formát exportu. Použij 'json' nebo 'xml'.");
    return;
}

await handler.RunAsync(input, output, format);

Console.WriteLine($"Export dokončen: {output}");
