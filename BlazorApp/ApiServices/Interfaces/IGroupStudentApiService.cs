using System;
using Application.Dtos.GroupStudentDto;
using BlazorApp.Dtos.GroupStudentDto;
using BlazorApp.DTOs.GroupStudentDto;

namespace BlazorApp.ApiServices.Interfaces;

public interface IGroupStudentApiService
{
   Task<List<GroupStudentResponseDto>?> GetActiveByGroupIdAsync(int groupId);
    Task<GroupStudentStatsDto?> GetGroupStatsAsync(int groupId);
    Task<List<GroupStudentHistoryDto>?> GetGroupHistoryAsync(int groupId);
    Task<List<GroupStudentResponseDto>?> GetByGroupIdAsync(int groupId);
    Task<GroupStudentResponseDto?> AddStudentAsync(AddStudentToGroupDto dto);
    Task<bool> RemoveStudentAsync(RemoveStudentFromGroupDto dto);
    Task<bool> TransferStudentAsync(TransferStudentDto dto);
    Task<GroupStudentResponseDto?> RestoreStudentAsync(int groupId, int studentId);
    Task<bool> BulkAddStudentsAsync(BulkAddStudentsDto dto);
}
