// BlazorApp/ApiServices/CourseApiService.cs
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorApp.ApiServices.Interfaces;
using BlazorApp.DTOs.CourseDto;

namespace BlazorApp.ApiServices;

public class CourseApiService : ICourseApiService
{
    private readonly HttpClient _http;

    public CourseApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CourseResponseDto>> GetAllAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<CourseResponseDto>>("api/courses") ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetAllAsync error: {ex.Message}");
            return new List<CourseResponseDto>();
        }
    }

    public async Task<List<CourseShortDto>> GetAllShortAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<CourseShortDto>>("api/courses/short") ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetAllShortAsync error: {ex.Message}");
            return new List<CourseShortDto>();
        }
    }

    public async Task<CourseResponseDto?> GetByIdAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<CourseResponseDto>($"api/courses/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetByIdAsync error: {ex.Message}");
            return null;
        }
    }

    public async Task<CourseResponseDto?> CreateAsync(CourseCreateDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/courses", dto);
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<CourseResponseDto>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            }
            
            Console.WriteLine($"CreateAsync failed: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CreateAsync error: {ex.Message}");
            return null;
        }
    }

    public async Task<CourseResponseDto?> UpdateAsync(int id, CourseUpdateDto dto)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"api/courses/{id}", dto);
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<CourseResponseDto>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            }
            
            Console.WriteLine($"UpdateAsync failed: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UpdateAsync error: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/courses/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DeleteAsync error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SetActiveAsync(int id, bool isActive)
    {
        try
        {
            var response = await _http.PatchAsync($"api/courses/{id}/set-active?isActive={isActive}", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SetActiveAsync error: {ex.Message}");
            return false;
        }
    }
}