using System.Globalization;
using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using ExpenseTrackerSharedCL.Features.Transactions.Enums;
using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Database.Models;
using ExpenseTrackerWebApi.Features.Transactions.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Extensions;

namespace ExpenseTrackerWebApi.Features.Transactions.Handlers
{
    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, TransactionsPageDataDto>
    {
        private readonly AppDbContext _context;

        public GetTransactionsQueryHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<TransactionsPageDataDto> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            var transctionsQuery = _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.TransactionTags)
                .ThenInclude(tt => tt.Tag)
            .Where(t => t.Account.IdentityUserId == request.UserId);

            var options = request.TransactionOptions;
            var filters = options.Filters;

            if (filters.TypeFilter is not null)
            {
                transctionsQuery = transctionsQuery.Where(t => t.Category.Type == (TransactionType)filters.TypeFilter);
            }

            if (filters.AccountsFilter is not null && filters.AccountsFilter.Count() != 0)
            {
                var accountIds = filters.AccountsFilter.Select(a => a.Id).ToList();
                transctionsQuery = transctionsQuery.Where(t => accountIds.Contains(t.AccountId));
            }

            if (filters.CategoriesFilter is not null && filters.CategoriesFilter.Count() != 0)
            {
                var categoryIds = filters.CategoriesFilter.Select(a => a.Id).ToList();
                transctionsQuery = transctionsQuery.Where(t => categoryIds.Contains(t.CategoryId));
            }

            if (filters.TagsFilter is not null && filters.TagsFilter.Count() != 0)
            {
                var tagIds = filters.TagsFilter.Select(t => t.Id).ToList();
                transctionsQuery = transctionsQuery.Where(t => t.TransactionTags.Any(tt => tagIds.Contains(tt.TagId)));
            }

            if (filters.DateFilter is not null)
            {
                switch (filters.DateFilter)
                {
                    case DateFilterPreset.ThisMonth:
                        var startOfMonth = DateTime.Now.StartOfMonth(CultureInfo.CurrentCulture);
                        transctionsQuery = transctionsQuery.Where(t => t.Date >= startOfMonth);
                        break;

                    case DateFilterPreset.LastMonth:
                        DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);
                        DateTime startOfLastMonth = oneMonthAgo.StartOfMonth(CultureInfo.CurrentCulture);
                        DateTime endOfLastMonth = oneMonthAgo.EndOfMonth(CultureInfo.CurrentCulture);
                        transctionsQuery = transctionsQuery.Where(t => t.Date >= startOfLastMonth && t.Date < endOfLastMonth);
                        break;

                    case DateFilterPreset.Last3Months:
                        DateTime threeMonthsAgo = DateTime.Now.AddMonths(-3);
                        DateTime startOf3MonthsAgoMonth = threeMonthsAgo.StartOfMonth(CultureInfo.CurrentCulture);
                        transctionsQuery = transctionsQuery.Where(t => t.Date >= startOf3MonthsAgoMonth);
                        break;


                    case DateFilterPreset.ThisYear:
                        var startOfyear = new DateTime(DateTime.Now.Year, 1, 1);
                        transctionsQuery = transctionsQuery.Where(t => t.Date >= startOfyear);
                        break;

                }
            }


            if (request.TransactionOptions.IsReoccuring)
            {
                transctionsQuery = transctionsQuery.Where(t => t.IsReoccuring != null && t.IsReoccuring == true);
            }
            else
            {
                transctionsQuery = transctionsQuery.Where(t => t.IsReoccuring == null || t.IsReoccuring == false);
            }

            if (!string.IsNullOrWhiteSpace(options.SortBy))
            {
                switch (options.SortBy)
                {
                    case nameof(TransactionDto.AccountDto):
                        transctionsQuery = options.SortDescending ? transctionsQuery.OrderByDescending(t => t.Account.Name) : transctionsQuery.OrderBy(t => t.Account.Name);
                        break;
                    case nameof(TransactionDto.CategoryDto):
                        transctionsQuery = options.SortDescending ? transctionsQuery.OrderByDescending(t => t.Category.Name) : transctionsQuery.OrderBy(t => t.Category.Name);
                        break;
                    case nameof(TransactionDto.NextReoccuranceDate):
                        transctionsQuery = options.SortDescending ? transctionsQuery.OrderByDescending(t => t.NextReoccuranceDate) : transctionsQuery.OrderBy(t => t.NextReoccuranceDate);
                        break;
                    case nameof(TransactionDto.DateStr):
                        transctionsQuery = options.SortDescending ? transctionsQuery.OrderByDescending(t => t.Date) : transctionsQuery.OrderBy(t => t.Date);
                        break;
                    case nameof(TransactionDto.Amount):
                        transctionsQuery = options.SortDescending ? transctionsQuery.OrderByDescending(t => t.Amount) : transctionsQuery.OrderBy(t => t.Amount);
                        break;
                    default:
                        transctionsQuery = transctionsQuery.OrderByDescending(t => t.Date);
                        break;
                }
            }
            else
            {
                transctionsQuery = transctionsQuery.OrderByDescending(t => t.Date);
            }

            int totalItems = await transctionsQuery.CountAsync(cancellationToken: cancellationToken);
            transctionsQuery = transctionsQuery.Skip(options.CurrentPage * options.PageSize).Take(options.PageSize);

            var transactions = (await transctionsQuery.ToListAsync(cancellationToken: cancellationToken)).Select(transaction => new TransactionDto()
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = transaction.Date,
                Time = transaction.Date.TimeOfDay,
                CategoryId = transaction.CategoryId,
                AccountId = transaction.AccountId,
                CategoryDto = new CategoryDto()
                {
                    Id = transaction.Category.Id,
                    Name = transaction.Category.Name,
                    Type = (TransactionTypeDto)transaction.Category.Type,
                    Color = transaction.Category.Color,
                    Icon = transaction.Category.Icon
                },
                AccountDto = new AccountDto()
                {
                    Id = transaction.Account.Id,
                    Name = transaction.Account.Name,
                    InitialBalance = transaction.Account.InitialBalance,
                    Icon = transaction.Account.Icon,
                    Color = transaction.Account.Color
                },
                TransactionType = (TransactionTypeDto)transaction.Category.Type,
                IsReoccuring = transaction.IsReoccuring,
                ReoccuranceFrequency = transaction.ReoccuranceFrequency != null ? (ReoccuranceFrequencyDto)transaction.ReoccuranceFrequency : null,
                NextReoccuranceDate = transaction.NextReoccuranceDate,
                TagIds = transaction.TransactionTags.Select(tt => tt.TagId).ToList(),
                TransactionTags = [.. transaction.TransactionTags.Select(tt => new TransactionTagDto()
                {
                    TagId = tt.TagId,
                    TransactionId = tt.TransactionId,
                    Tag = new TagDto()
                    {
                        Id = tt.Tag.Id,
                        Name = tt.Tag.Name,
                        Color = tt.Tag.Color
                    }
                })]
            }).ToList();

            return new TransactionsPageDataDto
            {
                Transactions = transactions,
                TotalItems = totalItems
            };
        }
    }
}