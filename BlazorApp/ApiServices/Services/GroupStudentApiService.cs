using System.Net.Http.Json;
using Application.Dtos.GroupStudentDto;
using BlazorApp.ApiServices.Interfaces;
using BlazorApp.Dtos.GroupStudentDto;
using BlazorApp.DTOs.GroupStudentDto;
namespace BlazorApp.ApiServices;
public class GroupStudentApiService : IGroupStudentApiService
{
    private readonly HttpClient _http;

    public GroupStudentApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<GroupStudentResponseDto>?> GetActiveByGroupIdAsync(int groupId)
    {
        var response = await _http.GetAsync($"api/group-students/group/{groupId}/active");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<List<GroupStudentResponseDto>>();
        return new List<GroupStudentResponseDto>();
    }

    public async Task<GroupStudentStatsDto?> GetGroupStatsAsync(int groupId)
    {
        var response = await _http.GetAsync($"api/group-students/group/{groupId}/stats");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<GroupStudentStatsDto>();
        return null;
    }

    public async Task<List<GroupStudentHistoryDto>?> GetGroupHistoryAsync(int groupId)
    {
        var response = await _http.GetAsync($"api/group-students/group/{groupId}/history");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<List<GroupStudentHistoryDto>>();
        return new List<GroupStudentHistoryDto>();
    }

    public async Task<List<GroupStudentResponseDto>?> GetByGroupIdAsync(int groupId)
    {
        var response = await _http.GetAsync($"api/group-students/group/{groupId}");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<List<GroupStudentResponseDto>>();
        return new List<GroupStudentResponseDto>();
    }

    public async Task<GroupStudentResponseDto?> AddStudentAsync(AddStudentToGroupDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/group-students/add", dto);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<GroupStudentResponseDto>();
        return null;
    }

    public async Task<bool> RemoveStudentAsync(RemoveStudentFromGroupDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/group-students/remove", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> TransferStudentAsync(TransferStudentDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/group-students/transfer", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<GroupStudentResponseDto?> RestoreStudentAsync(int groupId, int studentId)
    {
        var response = await _http.PostAsync($"api/group-students/restore/{groupId}/{studentId}", null);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<GroupStudentResponseDto>();
        return null;
    }

    public async Task<bool> BulkAddStudentsAsync(BulkAddStudentsDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/group-students/bulk-add", dto);
        return response.IsSuccessStatusCode;
    }
}