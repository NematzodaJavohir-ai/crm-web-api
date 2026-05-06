using System.Net.Http.Json;
using BlazorApp.ApiServices.Interfaces;
using BlazorApp.DTOs;
using BlazorApp.DTOs.StudentDto;

namespace BlazorApp.ApiServices;

public class StudentApiService(HttpClient http) : IStudentApiService
{
    public async Task<List<StudentResponseDto>> GetAllAsync()
    {
        var result = await http.GetFromJsonAsync<List<StudentResponseDto>>("api/students");
        return result ?? new List<StudentResponseDto>();
    }

    public async Task<StudentResponseDto?> GetByIdAsync(int id)
        => await http.GetFromJsonAsync<StudentResponseDto>($"api/students/{id}");

    public async Task<StudentFullProfileDto?> GetFullProfileAsync(int id)
        => await http.GetFromJsonAsync<StudentFullProfileDto>($"api/students/{id}/full-profile");

    public async Task<StudentResponseDto?> CreateAsync(StudentCreateDto dto)
    {
        var response = await http.PostAsJsonAsync("api/students", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<StudentResponseDto>();
    }

    public async Task<StudentResponseDto?> UpdateAsync(int id, StudentUpdateDto dto)
    {
        var response = await http.PutAsJsonAsync($"api/students/{id}", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<StudentResponseDto>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await http.DeleteAsync($"api/students/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SetActiveAsync(int id, bool isActive)
    {
        var response = await http.PatchAsync($"api/students/{id}/set-active?isActive={isActive}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<StudentResponseDto>> GetByGroupIdAsync(int groupId)
    {
        var result = await http.GetFromJsonAsync<List<StudentResponseDto>>($"api/students/group/{groupId}");
        return result ?? new List<StudentResponseDto>();
    }
}