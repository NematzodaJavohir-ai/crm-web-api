using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorApp.ApiServices.Interfaces;
using BlazorApp.DTOs.MentorDto;

namespace BlazorApp.ApiServices;

public class MentorService(HttpClient client) : IMentorApiService
{
    public async Task<List<MentorShortDto>> GetAllShortAsync()
{
    try
    {
        return await client.GetFromJsonAsync<List<MentorShortDto>>("api/mentors/short") ?? new();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"GetAllShortAsync error: {ex.Message}");
        return new List<MentorShortDto>();
    }
}
    public async Task<MentorResponseDto?> CreateAsync(MentorCreateDto dto)
    {
        try
        {
            var response = await client.PostAsJsonAsync("api/mentors", dto);
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<MentorResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                throw new Exception(errorResponse?.Message ?? errorResponse?.Error ?? $"API Error: {response.StatusCode}");
            }
            catch
            {
                throw new Exception($"Failed to create mentor: {content ?? response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Create mentor error: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await client.DeleteAsync($"api/mentors/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<MentorResponseDto>> GetAllActiveAsync()
    {
        var result = await client.GetFromJsonAsync<List<MentorResponseDto>>("api/mentors/active");
        return result ?? new List<MentorResponseDto>();
    }

    public async Task<List<MentorResponseDto>> GetAllAsync()
    {
        var result = await client.GetFromJsonAsync<List<MentorResponseDto>>("api/mentors");
        return result ?? new List<MentorResponseDto>();
    }

    public async Task<MentorResponseDto?> GetByIdAsync(int id)
        => await client.GetFromJsonAsync<MentorResponseDto>($"api/mentors/{id}");

    public async Task<MentorWithGroupsDto?> GetWithGroupsAsync(int id)
        => await client.GetFromJsonAsync<MentorWithGroupsDto>($"api/mentors/{id}/with-groups");

    public async Task<bool> SetActiveAsync(int id, bool isActive)
    {
        try
        {
            var response = await client.PatchAsync($"api/mentors/{id}/set-active?isActive={isActive}", null);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<MentorResponseDto?> UpdateMyProfileAsync(int userId, MentorUpdateDto dto)
    {
        try
        {
            var response = await client.PutAsJsonAsync($"api/mentors/profile/{userId}", dto);
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<MentorResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                throw new Exception(errorResponse?.Message ?? errorResponse?.Error ?? $"API Error: {response.StatusCode}");
            }
            catch
            {
                throw new Exception($"Failed to update mentor profile: {content ?? response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Update mentor profile error: {ex.Message}");
            throw;
        }
    }

    public async Task<MentorResponseDto?> UpdateAsync(int id, MentorUpdateDto dto)
    {
        try
        {
            var response = await client.PutAsJsonAsync($"api/mentors/{id}", dto);
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<MentorResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                throw new Exception(errorResponse?.Message ?? errorResponse?.Error ?? $"API Error: {response.StatusCode}");
            }
            catch
            {
                throw new Exception($"Failed to update mentor: {content ?? response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Update mentor error: {ex.Message}");
            throw;
        }
    }

    public async Task<MentorFullProfileDto?> GetFullProfileAsync(int id)
    {
        try
        {
            return await client.GetFromJsonAsync<MentorFullProfileDto>($"api/mentors/{id}/full-profile");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetFullProfileAsync error: {ex.Message}");
            return null;
        }
    }

    public async Task<MentorFullProfileDto?> GetMyFullProfileAsync()
    {
        try
        {
            return await client.GetFromJsonAsync<MentorFullProfileDto>("api/mentors/my-full-profile");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetMyFullProfileAsync error: {ex.Message}");
            return null;
        }
    }

    public async Task<MentorResponseDto?> GetMyProfileAsync()
    {
        try
        {
            return await client.GetFromJsonAsync<MentorResponseDto>("api/mentors/me");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetMyProfileAsync error: {ex.Message}");
            return null;
        }
    }
}

public class ErrorResponse
{
    public string? Message { get; set; }
    public string? Error { get; set; }
    public string? Title { get; set; }
    public int? Status { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}