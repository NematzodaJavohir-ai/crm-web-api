using System.Net.Http.Json;
using System.Text.Json;
using BlazorApp.ApiServices.Interfaces;
using BlazorApp.DTOs.LessonDto;

namespace BlazorApp.ApiServices;

public class LessonApiService : ILessonApiService
{
    private readonly HttpClient _http;
   public List<string> LastErrors { get; set; } = new();

    public LessonApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<LessonResponseDto>> GetAllAsync()
    {
        var result = await _http.GetFromJsonAsync<List<LessonResponseDto>>("api/lessons");
        return result ?? new();
    }

    public async Task<List<LessonResponseDto>> GetByGroupIdAsync(int groupId)
    {
        var result = await _http.GetFromJsonAsync<List<LessonResponseDto>>($"api/lessons/group/{groupId}");
        return result ?? new();
    }
    public async Task<LessonResponseDto?> GetByIdAsync(int id)
{
    return await _http.GetFromJsonAsync<LessonResponseDto>($"api/lessons/{id}");
}

    public async Task<List<LessonResponseDto>> GetByWeekNumberAsync(int groupId, int weekNumber)
    {
        var result = await _http.GetFromJsonAsync<List<LessonResponseDto>>($"api/lessons/group/{groupId}/week/{weekNumber}");
        return result ?? new();
    }

    public async Task<LessonWithAttendancesDto?> GetWithAttendancesAsync(int id)
    {
        return await _http.GetFromJsonAsync<LessonWithAttendancesDto>($"api/lessons/{id}/with-attendances");
    }

    public async Task<LessonResponseDto?> CreateAsync(LessonCreateDto dto)
    {
        LastErrors.Clear();
        var response = await _http.PostAsJsonAsync("api/lessons", dto);
        
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<LessonResponseDto>();
        
        await ParseErrors(response);
        return null;
    }

    public async Task<LessonResponseDto?> UpdateAsync(int id, LessonUpdateDto dto)
    {
        LastErrors.Clear();
        var response = await _http.PutAsJsonAsync($"api/lessons/{id}", dto);
        
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<LessonResponseDto>();
        
        await ParseErrors(response);
        return null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/lessons/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> MarkAsCompletedAsync(int id)
    {
        var response = await _http.PatchAsync($"api/lessons/{id}/complete", null);
        return response.IsSuccessStatusCode;
    }

    private async Task ParseErrors(HttpResponseMessage response)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync();
            
            // Пробуем распарсить как стандартную ошибку валидации
            try
            {
                var errorObj = JsonSerializer.Deserialize<ValidationErrorResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                if (errorObj?.Errors != null && errorObj.Errors.Any())
                {
                    foreach (var error in errorObj.Errors)
                    {
                        LastErrors.Add($"{error.Key}: {string.Join(", ", error.Value)}");
                    }
                }
                else if (!string.IsNullOrEmpty(errorObj?.Title))
                {
                    LastErrors.Add(errorObj.Title);
                }
                else
                {
                    LastErrors.Add(content);
                }
            }
            catch
            {
                LastErrors.Add(content);
            }
        }
        catch
        {
            LastErrors.Add($"Request failed with status: {response.StatusCode}");
        }
    }

    private class ValidationErrorResponse
    {
        public string? Title { get; set; }
        public int Status { get; set; }
        public Dictionary<string, List<string>>? Errors { get; set; }
    }
}