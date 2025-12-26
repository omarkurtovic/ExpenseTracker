using ExpenseTrackerSharedCL.Features.SharedKernel;
using ExpenseTrackerSharedCL.Features.UserPreferences.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace ExpenseTrackerSharedCL.Features.UserPreferences.Services
{
    public interface IUserPreferenceService
    {
        public Task<Result<UserPreferenceDto>> GetUserPreferencesAsync();

        public Task<Result> UpdateUserPreferencesAsync(UserPreferenceDto preferenceDto);

        public Task<Result> CreateDefaultUsrPreferencesAsync();
    }
}
