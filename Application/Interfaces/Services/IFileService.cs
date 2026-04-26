using System;
using Application.Results;
using Microsoft.AspNetCore.Http;
namespace Application.Interfaces.Services;

public interface IFileService
{ 
     Task<Result<string?>> UploadAsync(IFormFile file, string folderName);
    Task<Result<bool>> DeleteAsync(string filePath);
}


