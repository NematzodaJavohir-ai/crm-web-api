using BlazorApp.DTOs.LessonDto;

namespace BlazorApp.ApiServices.Interfaces;

public interface ILessonApiService
{
    List<string> LastErrors { get; set; }
    
    Task<List<LessonResponseDto>> GetAllAsync();
    Task<List<LessonResponseDto>> GetByGroupIdAsync(int groupId);
    Task<List<LessonResponseDto>> GetByWeekNumberAsync(int groupId, int weekNumber);
    Task<LessonResponseDto?> GetByIdAsync(int id);  
    Task<LessonWithAttendancesDto?> GetWithAttendancesAsync(int id);
    Task<LessonResponseDto?> CreateAsync(LessonCreateDto dto);
    Task<LessonResponseDto?> UpdateAsync(int id, LessonUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> MarkAsCompletedAsync(int id);
}