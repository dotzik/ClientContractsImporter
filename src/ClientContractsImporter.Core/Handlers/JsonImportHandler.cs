using ClientContractsImporter.Contracts.DataConverters;
using ClientContractsImporter.Contracts.Enums;
using ClientContractsImporter.Contracts.Exporters;
using ClientContractsImporter.Contracts.Handlers;
using Microsoft.Extensions.Logging;

namespace ClientContractsImporter.Core.Handlers;

public class JsonImportHandler : IImportHandler
{
    private readonly IClientDataConverter _converter;
    private readonly IEnumerable<IClientExporter> _exporters;
    private readonly ILogger<JsonImportHandler> _logger;

    public JsonImportHandler(
        ILogger<JsonImportHandler> logger,
        IClientDataConverter converter,
        IEnumerable<IClientExporter> exporters)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _converter = converter;
        _exporters = exporters;
    }

    public async Task RunAsync(
        string inputPath,
        string outputPath,
        ExportFormat format)
    {
        var exporter = _exporters.FirstOrDefault(e => e.Format == format);
        if (exporter == null)
        {
            _logger.LogError("Nepodporovaný formát exportu: {Format}", format);
            throw new InvalidOperationException($"Nepodporovaný formát: {format}");
        }

        await using var stream = File.OpenRead(inputPath);
        var clients = await _converter.ConvertAsync(stream);
        var bytes = await exporter.ExportAsync(clients);
        await File.WriteAllBytesAsync(outputPath, bytes);

        _logger.LogInformation("Export dokončen: {Path}", outputPath);
    }
}
