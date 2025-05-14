namespace ClientContractsImporter.Contracts.Enums;

public static class ExportFormatExtensions
{
    public static string GetExtension(this ExportFormat format) => format switch
    {
        ExportFormat.Json => ".json",
        ExportFormat.Xml => ".xml",
        _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
    };

    public static string GetContentType(this ExportFormat format) => format switch
    {
        ExportFormat.Json => "application/json",
        ExportFormat.Xml => "application/xml",
        _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
    };
}
