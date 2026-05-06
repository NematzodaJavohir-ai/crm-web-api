using System;
using BlazorApp.ApiServices.Enums;
using BlazorApp.DTOs.GroupDto;

namespace BlazorApp.ApiServices.Interfaces;

public interface IGroupApiService
{
   Task<List<GroupResponseDto>?> GetAllAsync();
    Task<List<GroupResponseDto>?> GetAllActiveAsync();
    Task<GroupResponseDto?> GetByIdAsync(int id);
    Task<GroupWithStudentsDto?> GetWithStudentsAsync(int id);
    Task<GroupResponseDto?> CreateAsync(GroupCreateDto dto);
    Task<GroupResponseDto?> UpdateAsync(int id, GroupUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ChangeStatusAsync(int id, GroupStatus status);
    Task<List<GroupResponseDto>?> GetByMentorIdAsync(int mentorId);
    Task<List<GroupResponseDto>?> GetByCourseIdAsync(int courseId);
}
