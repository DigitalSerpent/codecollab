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
            
            if (inner == null || string.IsNullOrEmpty(inner.Code))
                return Ok(new { error = "Нет кода для выполнения" });

            string tempFile = Path.GetTempFileName();
            string extension = "";
            string dockerImage = "";
            string dockerCommand = "";

            // Определяем язык
            switch (inner.Language.ToLower())
            {
                case "python":
                    extension = ".py";
                    dockerImage = "python:3.11-slim";
                    dockerCommand = "python3";
                    break;
                case "javascript":
                case "js":
                    extension = ".js";
                    dockerImage = "node:20-slim";
                    dockerCommand = "node";
                    break;
                case "bash":
                case "sh":
                    extension = ".sh";
                    dockerImage = "alpine";
                    dockerCommand = "sh";
                    break;
                default:
                    return Ok(new { error = $"Язык {inner.Language} временно не поддерживается. Доступны: python, javascript, bash" });
            }

            // Сохраняем код во временный файл
            string tempFileWithExt = tempFile + extension;
            await System.IO.File.WriteAllTextAsync(tempFileWithExt, inner.Code);

            // Делаем .sh файл исполняемым
            if (inner.Language.ToLower() == "bash" || inner.Language.ToLower() == "sh")
            {
                System.Diagnostics.Process.Start("chmod", $"+x {tempFileWithExt}").WaitForExit();
            }

            // Запускаем в Docker (без лишнего вывода)
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"run --rm -v {tempFileWithExt}:/app/code{extension} --pull=never {dockerImage} {dockerCommand} /app/code{extension} 2>/dev/null",
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
            
            // Удаляем временные файлы
            System.IO.File.Delete(tempFile);
            System.IO.File.Delete(tempFileWithExt);
            
            if (!string.IsNullOrEmpty(error))
                return Ok(new { error });
            
            return Ok(new { output });
        }
        catch (Exception ex)
        {
            return Ok(new { error = ex.Message });
        }
    }
}

public class CommandRequest
{
    public string Command { get; set; } = "";
}

public class InnerCommand
{
    public string Language { get; set; } = "";
    public string Code { get; set; } = "";
}