using BlazorApp.DTOs.AttendanceDto;

namespace BlazorApp.ApiServices.Interfaces;



public interface IAttendanceApiService
{
    Task<AttendanceResponseDto?> CreateAsync(AttendanceCreateDto dto);
    Task<AttendanceResponseDto?> UpdateAsync(int id, AttendanceUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<List<AttendanceResponseDto>> GetByLessonIdAsync(int lessonId);
    Task<List<AttendanceResponseDto>> GetByWeekAsync(int groupId, int weekNumber);
    Task<double> GetMyAverageScoreAsync(int groupId);
    Task<int> GetMyAbsenceCountAsync(int groupId);
    Task<List<AttendanceResponseDto>> GetMyAttendancesAsync(int groupId);
}