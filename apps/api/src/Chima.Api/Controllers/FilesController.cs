using Chima.Application.Common;
using Chima.Application.DTOs.Files;
using Chima.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chima.Api.Controllers;

[ApiController]
[Route("api/files")]
[Authorize]
public class FilesController : ControllerBase
{
    private const long MaxUploadBytes = 100 * 1024 * 1024;
    private readonly IFileService _files;
    private readonly ICurrentUser _current;

    public FilesController(IFileService files, ICurrentUser current)
    {
        _files = files;
        _current = current;
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var userId = _current.UserId ?? throw new UnauthorizedException();
        var files = await _files.ListAsync(userId, ct);
        return Ok(ApiResponse<IReadOnlyList<UploadedFileDto>>.Ok(files));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var userId = _current.UserId ?? throw new UnauthorizedException();
        var file = await _files.GetAsync(userId, id, ct)
                   ?? throw new NotFoundException("File not found.");
        return Ok(ApiResponse<UploadedFileDto>.Ok(file));
    }

    [HttpPost]
    [RequestSizeLimit(MaxUploadBytes)]
    [RequireQuota("file_upload")]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            throw new ValidationException("No file provided.");

        var userId = _current.UserId ?? throw new UnauthorizedException();

        await using var stream = file.OpenReadStream();
        var dto = await _files.UploadAsync(
            userId,
            stream,
            file.FileName,
            file.ContentType ?? "application/octet-stream",
            file.Length,
            ct);

        return Ok(ApiResponse<UploadedFileDto>.Ok(dto, "File uploaded. Processing in the background."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var userId = _current.UserId ?? throw new UnauthorizedException();
        await _files.DeleteAsync(userId, id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "File deleted."));
    }
}