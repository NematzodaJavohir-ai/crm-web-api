using System;
using BlazorApp.DTOs.StudentDto;

namespace BlazorApp.ApiServices.Interfaces;

public interface IStudentApiService
{
    Task<List<StudentResponseDto>> GetAllAsync();
    Task<StudentResponseDto?> GetByIdAsync(int id);
    Task<StudentFullProfileDto?> GetFullProfileAsync(int id);
    Task<StudentResponseDto?> CreateAsync(StudentCreateDto dto);
    Task<StudentResponseDto?> UpdateAsync(int id, StudentUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> SetActiveAsync(int id, bool isActive);
    Task<List<StudentResponseDto>> GetByGroupIdAsync(int groupId);
}
