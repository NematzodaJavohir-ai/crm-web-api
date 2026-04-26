using System;
using Application.Interfaces.Services;
using Application.Results;
using Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
namespace Infrastructure.Services;
public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<Result<string?>> UploadAsync(IFormFile file, string folderName)
    {
        try
        {
            if (file == null || file.Length == 0)
                return Result<string?>.Fail("File is empty", ErrorType.NotFound);

            var extension = Path.GetExtension(file.FileName).ToLower();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            if (!allowedExtensions.Contains(extension))
                return Result<string?>.Fail("Invalid file type", ErrorType.Validation);

            if (file.Length > 5 * 1024 * 1024)
                return Result<string?>.Fail("File too large (max 5MB)", ErrorType.Validation);

            var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + extension;
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("uploads", folderName, fileName).Replace("\\", "/");

            return Result<string?>.Ok("/" + relativePath);
        }
        catch (Exception ex)
        {
            return Result<string?>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public Task<Result<bool>> DeleteAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Task.FromResult(Result<bool>.Fail("Invalid file path"));

        var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));

        if (!File.Exists(fullPath))
            return Task.FromResult(Result<bool>.Fail("File not found"));

        File.Delete(fullPath);

        return Task.FromResult(Result<bool>.Ok(true));
    }
}
