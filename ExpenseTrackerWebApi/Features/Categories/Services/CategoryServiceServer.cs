using ExpenseTrackerSharedCL.Features.Categories;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerSharedCL.Features.SharedKernel;
using ExpenseTrackerWebApi.Features.Categories.Queries;
using MediatR;
using System.Security.Claims;

namespace ExpenseTrackerWebApi.Features.Categories.Services
{
    public class CategoryServiceServer : ICategoryService
    {
        private readonly ISender _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CategoryServiceServer(ISender mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<Result> CreateCategoryAsync(CategoryDto categoryDto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> EditCategoryAsync(CategoryDto categoryDto)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<CategoryDto>>> GetCategoriesAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = httpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var categories = await _mediator.Send(new GetCategoriesQuery { UserId = userId! });
                return Result<List<CategoryDto>>.Success(categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting categories: {ex.Message}");
                return Result<List<CategoryDto>>.Failure("An error occurred while fetching categories.");
            }
        }

        public async Task<Result<List<CategoryWithStatsDto>>> GetCategoriesWithStatsAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = httpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var categories = await _mediator.Send(new GetCategoriesWithStatsQuery { UserId = userId! });
                return Result<List<CategoryWithStatsDto>>.Success(categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting categories: {ex.Message}");
                return Result<List<CategoryWithStatsDto>>.Failure("An error occurred while fetching categories with stats.");
            }
        }
    }
}
