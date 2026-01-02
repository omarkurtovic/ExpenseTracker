using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Accounts.Commands;
using ExpenseTrackerWebApi.Features.Accounts.Queries;
using ExpenseTrackerWebApi.Features.Categories.Commands;
using ExpenseTrackerWebApi.Features.Categories.Queries;
using ExpenseTrackerWebApi.Features.DataSeeding.Commands;
using ExpenseTrackerWebApi.Features.Tags.Commands;
using ExpenseTrackerWebApi.Features.Tags.Models;
using ExpenseTrackerWebApi.Features.Tags.Queries;
using ExpenseTrackerWebApi.Features.Transactions.Models;
using MediatR;


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
            await _mediator.Send(new ResetToDefaultTagsCommand { UserId =  request.UserId }, cancellationToken);
            var accounts = await _mediator.Send(new GetAccountsQuery { UserId = request.UserId });
            var categories = await _mediator.Send(new GetCategoriesQuery { UserId = request.UserId });
            var tags = await _mediator.Send(new GetTagsQuery { UserId = request.UserId });

            var random = new Random();
            List<Transaction> transactions = new List<Transaction>();
            
            for (int i = 0; i < request.Options.NumberOfTransaction; i++)
            {
                var daysInterval = (DateTime)request.Options.TransactionEndDate! - (DateTime)request.Options.TransactionStartDate!;
                var daysAgo = random.Next(0, daysInterval.Days);
                var date = request.Options.TransactionStartDate!.Value.AddDays(daysAgo);

                var category = categories[random.Next(categories.Count)];
                var account = accounts[random.Next(accounts.Count)];
                var amount = random.Next((int)request.Options.TransactionMinAmount!, (int)request.Options.TransactionMaxAmount! + 1);
                if((TransactionType)category.Type! == TransactionType.Expense)
                {
                    amount = -amount;
                }

                var transaction = new Transaction
                {
                    Amount = amount,
                    Date = date.Date + new TimeSpan(random.Next(0, 24), random.Next(0, 60), 0),
                    AccountId = (int)account.Id!,
                    CategoryId = (int)category.Id!,
                    Description = GetRandomDescription(random)
                };
                transactions.Add(transaction);
            }


            _context.Transactions.AddRange(transactions);
            await _context.SaveChangesAsync();

            List<TransactionTag> transactionTags = new List<TransactionTag>();
            foreach(var transaction in transactions)
            {
                var tagCount = random.Next(0, (int)request.Options.MaxNumberOfTags! + 1);
                var assignedTags = new List<int>();
                for (int j = 0; j < tagCount; j++)
                {
                    var tag = tags[random.Next(tags.Count)];
                    if (!assignedTags.Contains((int)tag.Id!))
                    {
                        assignedTags.Add((int)tag.Id!);
                    }
                }
                foreach (var tagId in assignedTags)
                {
                    var tt = new TransactionTag
                    {
                        TransactionId = transaction.Id,
                        TagId = tagId
                    };
                    transactionTags.Add(tt);
                }
            }


            _context.TransactionTags.AddRange(transactionTags);
            await _context.SaveChangesAsync();
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