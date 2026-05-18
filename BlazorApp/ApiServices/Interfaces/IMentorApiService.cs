using System;
using BlazorApp.DTOs.MentorDto;

namespace BlazorApp.ApiServices.Interfaces;



public interface IMentorApiService
{
    // Основные CRUD
    Task<List<MentorResponseDto>> GetAllAsync();
    Task<List<MentorResponseDto>> GetAllActiveAsync();
    Task<MentorResponseDto?> GetByIdAsync(int id);
    Task<MentorResponseDto?> CreateAsync(MentorCreateDto dto);
    Task<MentorResponseDto?> UpdateAsync(int id, MentorUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> SetActiveAsync(int id, bool isActive);
    
    Task<MentorWithGroupsDto?> GetWithGroupsAsync(int id);
    Task<MentorFullProfileDto?> GetFullProfileAsync(int id);           // ← ДОБАВИТЬ
    Task<MentorFullProfileDto?> GetMyFullProfileAsync();               // ← ДОБАВИТЬ
    

    Task<MentorResponseDto?> GetMyProfileAsync();
    Task<List<MentorShortDto>> GetAllShortAsync();
    Task<MentorResponseDto?> UpdateMyProfileAsync(int userId, MentorUpdateDto dto);
} 

