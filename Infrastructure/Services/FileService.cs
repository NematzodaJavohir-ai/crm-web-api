using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRoot, "uploads", folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + extension;
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

    public async Task<Result<bool>> DeleteAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return Result<bool>.Fail("Invalid file path", ErrorType.Validation);

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var fullPath = Path.Combine(webRoot, filePath.TrimStart('/'));

            if (!File.Exists(fullPath))
                return Result<bool>.Fail("File not found", ErrorType.NotFound);

            File.Delete(fullPath);

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }
}