using ClientContractsImporter.Contracts.Enums;

namespace ClientContractsImporter.Contracts.Handlers;

public interface IImportHandler
{
    Task RunAsync(
        string inputPath,
        string outputPath,
        ExportFormat format);
}
