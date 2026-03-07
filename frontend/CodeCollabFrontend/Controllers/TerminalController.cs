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
            string dockerCommand = "";
            string dockerImage = "code-runner"; // наш универсальный образ

            // Определяем язык
            switch (inner.Language.ToLower())
            {
                case "python":
                    extension = ".py";
                    dockerCommand = "python3";
                    break;
                case "javascript":
                case "js":
                    extension = ".js";
                    dockerCommand = "node";
                    break;
                case "typescript":
                case "ts":
                    extension = ".ts";
                    dockerCommand = "ts-node";
                    break;
                case "bash":
                case "sh":
                    extension = ".sh";
                    dockerCommand = "bash";
                    break;
                case "ruby":
                case "rb":
                    extension = ".rb";
                    dockerCommand = "ruby";
                    break;
                case "java":
                    extension = ".java";
                    dockerCommand = "java";
                    break;
                case "c":
                case "cpp":
                case "c++":
                    extension = ".cpp";
                    dockerCommand = "g++ -o /app/a.out /app/code.cpp && /app/a.out";
                    break;
                case "go":
                case "golang":
                    extension = ".go";
                    dockerCommand = "go run";
                    break;
                case "php":
                    extension = ".php";
                    dockerCommand = "php";
                    break;
                case "csharp":
                case "cs":
                    extension = ".cs";
                    dockerCommand = "dotnet script";
                    break;
                default:
                    return Ok(new { error = $"Язык {inner.Language} не поддерживается" });
            }

            // Сохраняем код во временный файл
            string tempFileWithExt = tempFile + extension;
            await System.IO.File.WriteAllTextAsync(tempFileWithExt, inner.Code);

            // Запускаем в Docker
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"run --rm -v {tempFileWithExt}:/app/code{extension} {dockerImage} {dockerCommand} /app/code{extension}",
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