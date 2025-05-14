using ClientContractsImporter.Contracts.DataConverters;
using ClientContractsImporter.Contracts.Models;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;

namespace ClientContractsImporter.Core.DataConverters;

public class XlsxClientDataConverter : IClientDataConverter
{
    private readonly ILogger<XlsxClientDataConverter> _logger;

    public XlsxClientDataConverter(ILogger<XlsxClientDataConverter> logger)
    {
        _logger = logger;
    }

    public async Task<List<Client>> ConvertAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanRead)
        {
            throw new ArgumentException("Stream musí podporovat čtení", nameof(stream));
        }

        await CheckXlsxFileFormat(stream, cancellationToken);

        return await Task.Run(
            () =>
            {
                var clients = new List<Client>();

                using (var workbook = new XLWorkbook(stream))
                {
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        var startCol = 4;
                        var endCol = worksheet.LastColumnUsed()?.ColumnNumber() ?? 3;

                        var periods = worksheet.Row(1)
                            .Cells(startCol, endCol)
                            .Select(c => c.GetFormattedString().Trim()) // retuns formatted string XX/YYYY
                            .ToList();

                        foreach (var row in worksheet.RowsUsed().Skip(1))
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var parsed = Parse(row, periods);
                            if (parsed is null)
                            {
                                continue;
                            }

                            var (name, ic, contractName, records) = parsed.Value;

                            MapRow(clients, name, ic, contractName, records);
                        }

                        _logger.LogInformation(
                            "List '{Sheet}': načteno {Count} klientů.",
                            worksheet.Name,
                            clients.Count);
                    }
                }

                return clients;
            },
            cancellationToken);
    }

    private async Task CheckXlsxFileFormat(Stream stream, CancellationToken cancellationToken)
    {
        try
        {
            // Implemnt xlsx file format check
            var headerBytes = new byte[4];
            var originalPosition = stream.Position;
            await stream.ReadExactlyAsync(headerBytes.AsMemory(0, 4), cancellationToken);
            stream.Position = originalPosition; // Work with memory return to original position!

            // Check XLSX file signature
            if (!IsValidXlsxHeader(headerBytes))
            {
                throw new InvalidDataException("Vstupní stream neobsahuje platný XLSX soubor");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Nepodařilo se validovat vstupní soubor");
            throw new InvalidOperationException("Nepodařilo se zpracovat vstupní soubor XLSX", ex);
        }
    }

    private bool IsValidXlsxHeader(byte[] header)
    {
        // XLSX files are actually ZIP files, so we check the ZIP file signature
        return header[0] == 0x50 && header[1] == 0x4B && header[2] == 0x03 && header[3] == 0x04;
    }

    private (string Name, string CompanyId, string ContractName, List<ProductionRecord> Records)? Parse(
        IXLRow row,
        List<string> periods)
    {
        var name = row.Cell(1).GetString().Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var ic = row.Cell(2).GetString().Trim();
        var contractName = row.Cell(3).GetString().Trim();

        if (string.IsNullOrWhiteSpace(ic) || string.IsNullOrWhiteSpace(contractName))
        {
            _logger.LogWarning("Řádek {Row}: chybí IČ nebo název zakázky.", row.RowNumber());
            return null;
        }

        var records = new List<ProductionRecord>();

        for (int i = 0; i < periods.Count; i++)
        {
            var period = periods[i];
            var cell = row.Cell(i + 4); // 1-based index

            int? quantity = null;
            try
            {
                quantity = cell.GetValue<int?>();
            }
            catch
            {
                _logger.LogWarning("Řádek {Row}, sloupec {Col}: nelze převést hodnotu na int.", row.RowNumber(), i + 4);
            }

            records.Add(new ProductionRecord(period, quantity));
        }

        return (name, ic, contractName, records);
    }

    private void MapRow(
        List<Client> clients,
        string name,
        string companyId,
        string contractName,
        List<ProductionRecord> records)
    {
        var client = clients.FirstOrDefault(c => c.Name == name && c.CompanyId == companyId);
        if (client is null)
        {
            client = new Client(name, companyId);
            clients.Add(client);
        }

        var contract = new Contract(contractName);
        contract.Records.AddRange(records);

        client.Contracts.Add(contract);
    }
}
