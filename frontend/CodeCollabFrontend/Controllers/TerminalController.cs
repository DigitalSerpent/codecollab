using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
namespace CodeCollabFrontend.Controllers;
[ApiController]
[Route("api/[controller]")]
public class TerminalController : ControllerBase
{
    [HttpPost("run")]
    public async Task<IActionResult> RunCommand([FromBody] CommandRequest request)
    {
        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var inner = JsonSerializer.Deserialize<InnerCommand>(request.Command, options);
            if (inner == null || string.IsNullOrEmpty(inner.Code)) return Ok(new { error = "Нет кода" });
            string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            string ext = inner.Language.ToLower() switch { "python" => ".py", "javascript" or "js" => ".js", "bash" or "sh" => ".sh", _ => "" };
            if (string.IsNullOrEmpty(ext)) return Ok(new { error = $"Язык {inner.Language} не поддерживается" });
            string fileWithExt = tempFile + ext;
            await System.IO.File.WriteAllTextAsync(fileWithExt, inner.Code);
            string dockerCmd = inner.Language.ToLower() switch { "python" => "python3", "javascript" or "js" => "node", _ => "bash" };
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"run --rm -v {fileWithExt}:/app/code{ext} code-collab-runner {dockerCmd} /app/code{ext}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            if (System.IO.File.Exists(fileWithExt)) System.IO.File.Delete(fileWithExt);
            return Ok(new { output = string.IsNullOrEmpty(error) ? output : error });
        }
        catch (Exception ex) { return Ok(new { error = ex.Message }); }
    }
}
public class CommandRequest { public string Command { get; set; } = ""; }
public class InnerCommand { public string Language { get; set; } = ""; public string Code { get; set; } = ""; }
