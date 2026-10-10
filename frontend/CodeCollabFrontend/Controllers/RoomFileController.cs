using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeCollabFrontend.Models;
namespace CodeCollabFrontend.Controllers;
[ApiController]
[Route("api/room/{roomId}/file")]
public class RoomFileController : ControllerBase
{
    private readonly AppDbContext _context;
    public RoomFileController(AppDbContext context) { _context = context; }
    [HttpGet]
    public async Task<IActionResult> GetFiles(int roomId)
    {
        return Ok(await _context.RoomFiles.Where(f => f.RoomId == roomId).Select(f => new { f.Id, f.Name, f.IsReadme }).ToListAsync());
    }
    [HttpGet("{fileId}")]
    public async Task<IActionResult> GetFile(int roomId, int fileId)
    {
        var file = await _context.RoomFiles.FirstOrDefaultAsync(f => f.Id == fileId && f.RoomId == roomId);
        return file == null ? NotFound() : Ok(new { file.Id, file.Name, file.Content, file.IsReadme });
    }
    [HttpPost]
    public async Task<IActionResult> CreateFile(int roomId, [FromBody] CreateFileRequest request)
    {
        if (!await _context.Rooms.AnyAsync(r => r.Id == roomId)) return NotFound("Комната не найдена");
        if (await _context.RoomFiles.AnyAsync(f => f.RoomId == roomId && f.Name == request.Name)) return Conflict("Файл существует");
        var newFile = new RoomFile { RoomId = roomId, Name = request.Name, Content = "", IsReadme = request.Name.ToLower() == "readme.md" };
        _context.RoomFiles.Add(newFile);
        await _context.SaveChangesAsync();
        return Ok(new { newFile.Id, newFile.Name, newFile.IsReadme });
    }
    [HttpPut("{fileId}")]
    public async Task<IActionResult> UpdateFile(int roomId, int fileId, [FromBody] UpdateFileRequest request)
    {
        var file = await _context.RoomFiles.FirstOrDefaultAsync(f => f.Id == fileId && f.RoomId == roomId);
        if (file == null) return NotFound();
        file.Content = request.Content;
        await _context.SaveChangesAsync();
        return Ok();
    }
    [HttpPut("{fileId}/rename")]
    public async Task<IActionResult> RenameFile(int roomId, int fileId, [FromBody] RenameFileRequest request)
    {
        var file = await _context.RoomFiles.FirstOrDefaultAsync(f => f.Id == fileId && f.RoomId == roomId);
        if (file == null) return NotFound();
        if (await _context.RoomFiles.AnyAsync(f => f.RoomId == roomId && f.Name == request.NewName && f.Id != fileId)) return Conflict("Имя занято");
        file.Name = request.NewName;
        await _context.SaveChangesAsync();
        return Ok();
    }
    [HttpDelete("{fileId}")]
    public async Task<IActionResult> DeleteFile(int roomId, int fileId)
    {
        var file = await _context.RoomFiles.FirstOrDefaultAsync(f => f.Id == fileId && f.RoomId == roomId);
        if (file == null) return NotFound();
        _context.RoomFiles.Remove(file);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
public class CreateFileRequest { public string Name { get; set; } = ""; }
public class UpdateFileRequest { public string Content { get; set; } = ""; }
public class RenameFileRequest { public string NewName { get; set; } = ""; }
