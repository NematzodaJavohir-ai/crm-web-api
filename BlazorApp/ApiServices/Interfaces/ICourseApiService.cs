using System;
using BlazorApp.DTOs.CourseDto;

namespace BlazorApp.ApiServices.Interfaces;

public interface ICourseApiService
{
   Task<List<CourseResponseDto>> GetAllAsync();
    Task<List<CourseShortDto>> GetAllShortAsync();
    Task<CourseResponseDto?> GetByIdAsync(int id);
    Task<CourseResponseDto?> CreateAsync(CourseCreateDto dto);
    Task<CourseResponseDto?> UpdateAsync(int id, CourseUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> SetActiveAsync(int id, bool isActive);
}
