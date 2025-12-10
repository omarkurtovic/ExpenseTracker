using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;
using ExpenseTrackerSharedCL.Features.Transactions.Enums;

namespace ExpenseTrackerSharedCL.Features.Transactions.Dtos
{
    public class TransactionsFiltersDto
    {
        public TransactionTypeDto? TypeFilter{get; set;}
        public DateFilterPreset? DateFilter{get; set;}
        public IEnumerable<AccountDto> AccountsFilter{get; set;} = [];
        public IEnumerable<CategoryDto> CategoriesFilter{get; set;} = [];
        public IEnumerable<TagDto> TagsFilter{get; set;} = [];

        public void ClearFilters()
        {
            TypeFilter = null;
            DateFilter = null;
            AccountsFilter = [];
            CategoriesFilter = [];
            TagsFilter = [];
        }

        public bool AreAnyFiltersActive()
        {
            return TypeFilter != null ||
                   DateFilter != null ||
                   AccountsFilter.Any() ||
                   CategoriesFilter.Any() ||
                   TagsFilter.Any();
        }

        
    }
}