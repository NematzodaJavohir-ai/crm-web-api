using System.Net.Http.Json;
using BlazorApp.ApiServices.Interfaces;
using BlazorApp.DTOs.AttendanceDto;

namespace BlazorApp.ApiServices;

public class AttendanceApiService(HttpClient http) : IAttendanceApiService
{
    public async Task<AttendanceResponseDto?> CreateAsync(AttendanceCreateDto dto)
    {
        var response = await http.PostAsJsonAsync("api/attendances", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AttendanceResponseDto>();
    }

    public async Task<AttendanceResponseDto?> UpdateAsync(int id, AttendanceUpdateDto dto)
    {
        var response = await http.PutAsJsonAsync($"api/attendances/{id}", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AttendanceResponseDto>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await http.DeleteAsync($"api/attendances/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<AttendanceResponseDto>> GetByLessonIdAsync(int lessonId)
    {
        var result = await http.GetFromJsonAsync<List<AttendanceResponseDto>>($"api/attendances/lesson/{lessonId}");
        return result ?? new();
    }

    public async Task<List<AttendanceResponseDto>> GetByWeekAsync(int groupId, int weekNumber)
    {
        var result = await http.GetFromJsonAsync<List<AttendanceResponseDto>>($"api/attendances/week?groupId={groupId}&weekNumber={weekNumber}");
        return result ?? new();
    }

    public async Task<double> GetMyAverageScoreAsync(int groupId)
    {
        var result = await http.GetFromJsonAsync<double>($"api/attendances/me/group/{groupId}/average-score");
        return result;
    }

    public async Task<int> GetMyAbsenceCountAsync(int groupId)
    {
        var result = await http.GetFromJsonAsync<int>($"api/attendances/me/group/{groupId}/absence-count");
        return result;
    }

    public async Task<List<AttendanceResponseDto>> GetMyAttendancesAsync(int groupId)
    {
        var result = await http.GetFromJsonAsync<List<AttendanceResponseDto>>($"api/attendances/me/group/{groupId}");
        return result ?? new();
    }
}