using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerSharedCL.Features.SharedKernel;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace ExpenseTrackerSharedCL.Features.Categories
{
    public interface ICategoryService
    {
        public Task<Result<List<CategoryDto>>> GetCategoriesAsync();

        public Task<Result<List<CategoryWithStatsDto>>> GetCategoriesWithStatsAsync();

        public Task<Result> CreateCategoryAsync(CategoryDto categoryDto);
        public Task<Result> EditCategoryAsync(CategoryDto categoryDto);

        public Task<Result> DeleteCategoryAsync(int categoryId);
    }
}
