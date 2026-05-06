using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorApp.ApiServices.Interfaces;
using BlazorApp.DTOs.GroupDto;
using BlazorApp.DTOs.StudentDto;
using BlazorApp.ApiServices.Enums;

namespace BlazorApp.ApiServices;

public class GroupApiService( HttpClient _http) : IGroupApiService
{
    

    public async Task<List<GroupResponseDto>?> GetAllAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<GroupResponseDto>>("api/groups");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetAllAsync error: {ex.Message}");
            return new List<GroupResponseDto>();
        }
    }

    public async Task<List<GroupResponseDto>?> GetAllActiveAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<GroupResponseDto>>("api/groups/active");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetAllActiveAsync error: {ex.Message}");
            return new List<GroupResponseDto>();
        }
    }

    public async Task<GroupResponseDto?> GetByIdAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<GroupResponseDto>($"api/groups/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetByIdAsync error: {ex.Message}");
            return null;
        }
    }

    public async Task<GroupWithStudentsDto?> GetWithStudentsAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<GroupWithStudentsDto>($"api/groups/{id}/with-students");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetWithStudentsAsync error: {ex.Message}");
            return null;
        }
    }

    public async Task<GroupResponseDto?> CreateAsync(GroupCreateDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/groups", dto);
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<GroupResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            
            throw new Exception($"Failed to create group: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CreateAsync error: {ex.Message}");
            throw;
        }
    }

    public async Task<GroupResponseDto?> UpdateAsync(int id, GroupUpdateDto dto)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"api/groups/{id}", dto);
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<GroupResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            
            throw new Exception($"Failed to update group: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UpdateAsync error: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/groups/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DeleteAsync error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ChangeStatusAsync(int id, GroupStatus status)
    {
        try
        {
            var response = await _http.PatchAsync($"api/groups/{id}/status?status={status}", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ChangeStatusAsync error: {ex.Message}");
            return false;
        }
    }

    public async Task<List<GroupResponseDto>?> GetByMentorIdAsync(int mentorId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<GroupResponseDto>>($"api/groups/mentor/{mentorId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetByMentorIdAsync error: {ex.Message}");
            return new List<GroupResponseDto>();
        }
    }

    public async Task<List<GroupResponseDto>?> GetByCourseIdAsync(int courseId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<GroupResponseDto>>($"api/groups/course/{courseId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetByCourseIdAsync error: {ex.Message}");
            return new List<GroupResponseDto>();
        }
    }
}