using System.Diagnostics;
using ClientContractsImporter.Contracts.Enums;
using ClientContractsImporter.Contracts.Handlers;
using ClientContractsImporter.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClientContractsImporter.Web.Controllers;
public class HomeController : Controller
{
    private readonly IImportHandler _handler;

    public HomeController(IImportHandler handler)
    {
        _handler = handler;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, ExportFormat format)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Neplatný soubor");
        }

        var tempInput = Path.GetTempFileName();
        var tempOutput = Path.ChangeExtension(Path.GetTempFileName(), format.GetExtension());

        await using (var fileStream = System.IO.File.Create(tempInput))
        {
            await file.CopyToAsync(fileStream);
        }

        await _handler.RunAsync(tempInput, tempOutput, format);

        var outputBytes = await System.IO.File.ReadAllBytesAsync(tempOutput);
        var contentType = format.GetContentType();
        return File(outputBytes, contentType, Path.GetFileName(tempOutput));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
