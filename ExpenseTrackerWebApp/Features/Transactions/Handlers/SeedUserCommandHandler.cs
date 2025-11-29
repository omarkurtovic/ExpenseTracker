using MediatR;
using ExpenseTrackerWebApp.Features.Transactions.Commands;
using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using MudBlazor;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerWebApp.Features.SharedKernel.Commands;
using ExpenseTrackerWebApp.Features.SharedKernel.Queries;

namespace ExpenseTrackerWebApp.Features.Transactions.Handlers{
    public class SeedUserCommandHandler : IRequestHandler<SeedUserCommand>
    {
        private readonly AppDbContext _context;
        private readonly ISender _mediator;

        public SeedUserCommandHandler(AppDbContext context, ISender mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task Handle(SeedUserCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ResetToDefaultAccountsCommand { UserId =  request.UserId }, cancellationToken);
            await _mediator.Send(new ResetToDefaultCategoriesCommand { UserId =  request.UserId }, cancellationToken);
            var accounts = await _mediator.Send(new GetAccountsQuery { UserId = request.UserId });
            var categories = await _mediator.Send(new GetCategoriesQuery { UserId = request.UserId });

            var tags = await CreateSeedTagsAsync(request.UserId);
            var random = new Random();
            var today = DateTime.Today;
            
            for (int i = 0; i < 2000; i++)
            {
                var daysAgo = random.Next(0, 1200);
                var date = today.AddDays(-daysAgo);

                var category = categories[random.Next(categories.Count)];
                var account = accounts[random.Next(accounts.Count)];
                var amount = random.Next(10, 500);
                if(category.Type == TransactionType.Expense)
                {
                    amount = -amount;
                }

                var tagCount = random.Next(0, 4);
                var assignedTags = new List<int>();
                for (int j = 0; j < tagCount; j++)
                {
                    var tag = tags[random.Next(tags.Count)];
                    if (!assignedTags.Contains(tag.Id))
                    {
                        assignedTags.Add(tag.Id);
                    }
                }

                var transaction = new Transaction
                {
                    Amount = amount,
                    Date = date.Date + new TimeSpan(random.Next(0, 24), random.Next(0, 60), 0),
                    AccountId = account.Id,
                    CategoryId = category.Id,
                    Description = GetRandomDescription(random)
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                foreach (var tagId in assignedTags)
                {
                    _context.TransactionTags.Add(new TransactionTag
                    {
                        TransactionId = transaction.Id,
                        TagId = tagId
                    });
                }
                await _context.SaveChangesAsync();
            }
        }
        
        private async Task<List<Tag>> CreateSeedTagsAsync(string userId)
        {
            var seedTags = new List<(string name, string color)>
            {
                ("Groceries", "#4CAF50"),       
                ("Dining Out", "#FF9800"),      
                ("Shopping", "#E91E63"),        
                ("Utilities", "#2196F3"),       
                ("Entertainment", "#9C27B0"),   
                ("Transportation", "#00BCD4"),  
                ("Health", "#F44336"),          
                ("Work Related", "#FFC107"),    
                ("Subscription", "#795548"),    
                ("Emergency", "#607D8B")        
            };

            var tags = new List<Tag>();
            foreach (var (name, color) in seedTags)
            {
                var tag = new Tag(){Name = name, Color = color, IdentityUserId = userId};
                tags.Add(tag);
            }
            _context.Tags.AddRange(tags);
            await _context.SaveChangesAsync();
            return tags;
        }
        private static string? GetRandomDescription(Random random)
        {
            var descriptions = new List<string?>()
            {
                "Supermarket", "Whole Foods", "Trader Joe's", "Weekly groceries",
                "Lunch", "Dinner with friends", "Coffee shop", "Fast food",
                "Clothing", "Electronics", "Home goods", "Amazon order",
                "Gas", "Uber", "Public transit", "Parking",
                "Car maintenance", "Oil change", "Car wash", "Insurance",
                "Phone bill", "Internet", "Streaming services",
                "Pharmacy", "Doctor visit", "Gym membership", "Vitamins",
                "Course fees", "Books", "Online class",
                "Movie tickets", "Concert", "Games", "Hobby supplies",
                "Pet food", "Vet visit", "Pet supplies", null 
            };

            return descriptions[random.Next(descriptions.Count)];
        }
    
    }
}