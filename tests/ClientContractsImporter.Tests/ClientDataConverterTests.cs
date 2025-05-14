using System.Text;
using ClientContractsImporter.Core.DataConverters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ClientContractsImporter.Tests;

public class ClientDataConverterTests
{
    [Fact]
    public async Task ConvertAsync_ShouldParseValidXlsxFile()
    {
        var filePath = Path.Combine("TestData", "test.xlsx");
        Assert.True(File.Exists(filePath), "Testovací soubor 'test.xlsx' nebyl nalezen.");

        await using var stream = File.OpenRead(filePath);
        var converter = new XlsxClientDataConverter(new NullLogger<XlsxClientDataConverter>());

        var clients = await converter.ConvertAsync(stream);

        Assert.NotNull(clients);
        Assert.All(clients, client =>
        {
            Assert.False(string.IsNullOrWhiteSpace(client.Name));
            Assert.False(string.IsNullOrWhiteSpace(client.CompanyId));
            Assert.NotEmpty(client.Contracts);
        });
    }

    [Fact]
    public async Task ConvertAsync_WithEmptyStream_ThrowsInvalidOperationException()
    {
        // Arrange
        var emptyStream = new MemoryStream();
        var converter = new XlsxClientDataConverter(new NullLogger<XlsxClientDataConverter>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => converter.ConvertAsync(emptyStream));

        // Dodatečná kontrola vnitřní výjimky
        Assert.Contains("Nepodařilo se zpracovat vstupní soubor XLSX", exception.Message);
        Assert.IsType<EndOfStreamException>(exception.InnerException);
    }

    [Fact]
    public async Task ConvertAsync_WithNullStream_ThrowsArgumentNullException()
    {
        // Arrange
        var converter = new XlsxClientDataConverter(new NullLogger<XlsxClientDataConverter>());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => converter.ConvertAsync(null));
    }

    [Fact]
    public async Task ConvertAsync_WithInvalidFile_LogsWarnings()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<XlsxClientDataConverter>>();
        var converter = new XlsxClientDataConverter(mockLogger.Object);

        using var invalidStream = new MemoryStream(Encoding.UTF8.GetBytes("This is not an XLSX file"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => converter.ConvertAsync(invalidStream));

        // Verify that warnings were logged
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Nepodařilo se validovat")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
