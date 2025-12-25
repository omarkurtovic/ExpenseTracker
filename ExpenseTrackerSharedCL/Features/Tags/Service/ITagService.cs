using ExpenseTrackerSharedCL.Features.SharedKernel;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace ExpenseTrackerSharedCL.Features.Tags.Service
{
    public interface ITagService
    {
        public Task<Result<List<TagDto>>> GetTagsAsync();

        public Task<Result<TagDto>> CreateTagAsync(TagDto tagDto);

        public Task<Result> DeleteTagAsync(int tagId);
    }
}
