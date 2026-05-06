using System;
using BlazorApp.DTOs.MentorDto;

namespace BlazorApp.ApiServices.Interfaces;


public interface IMentorApiService
{
    Task<List<MentorResponseDto>> GetAllAsync();
    Task<List<MentorResponseDto>> GetAllActiveAsync();
    Task<MentorResponseDto?> GetByIdAsync(int id);
    Task<MentorWithGroupsDto?> GetWithGroupsAsync(int id);
    Task<MentorResponseDto?> CreateAsync(MentorCreateDto dto);
    Task<MentorResponseDto?> UpdateMyProfileAsync(int userId, MentorUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> SetActiveAsync(int id, bool isActive);
     Task<MentorResponseDto?> UpdateAsync(int id, MentorUpdateDto dto);  
}
