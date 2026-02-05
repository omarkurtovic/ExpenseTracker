using ExpenseTrackerSharedCL.Features.SharedKernel;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;
using ExpenseTrackerSharedCL.Features.Tags.Service;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ExpenseTrackerWasmWebApp.Features.Tags.Services;

public class TagService(HttpClient httpClient) : ITagService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<Result<List<TagDto>>> GetTagsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/tags");
            if (response.IsSuccessStatusCode)
            {
                var tags = await response.Content.ReadFromJsonAsync<List<TagDto>>() ?? new();
                return Result<List<TagDto>>.Success(tags);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Result<List<TagDto>>.Failure("Unauthorized access.", FailureReason.Unauthorized);
            }

            Console.WriteLine($"Error fetching tags! Status Code: {response.StatusCode}!");
            return Result<List<TagDto>>.Failure("Failed to fetch tags.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Result<List<TagDto>>.Failure("An error occurred while fetching tags.");
        }
    }

    public async Task<Result<TagDto>> CreateTagAsync(TagDto tagDto)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"api/tags");
            request.Content = JsonContent.Create(tagDto);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var created = await response.Content.ReadFromJsonAsync<TagDto>();
                tagDto.Id = created!.Id;
                return Result<TagDto>.Success(tagDto!);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Result<TagDto>.Failure("Unauthorized access.", FailureReason.Unauthorized);
            }

            Console.WriteLine($"Error creating tag! Status Code: {response.StatusCode}!");
            return Result<TagDto>.Failure("Failed to create tag.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Result<TagDto>.Failure("An error occurred while creating the tag.");
        }
    }

    public async Task<Result> DeleteTagAsync(int tagId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/tags/{tagId}");
            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Result.Failure("Unauthorized access.", FailureReason.Unauthorized);
            }

            Console.WriteLine($"Error deleting tag! Status Code: {response.StatusCode}!");
            return Result.Failure("Failed to delete tag.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Result.Failure("An error occurred while deleting the tag.");
        }
    }
}
