using ExpenseTrackerWebApp.Database.Models;
using MediatR;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Queries
{
    public class GetCategoriesQuery : IRequest<List<Category>>
    {
        public string UserId{get; set; }
    }
}