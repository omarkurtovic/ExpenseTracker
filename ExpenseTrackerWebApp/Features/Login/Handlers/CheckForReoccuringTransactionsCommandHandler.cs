using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Login.Commands;
using ExpenseTrackerWebApp.Features.SharedKernel.Queries;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApp.Features.Login.Handlers
{
    public class CheckForReoccuringTransactionsCommandHandler : IRequestHandler<CheckForReoccuringTransactionsCommand>
    {
        private readonly AppDbContext _context;
        private readonly ISender _mediator;

        public CheckForReoccuringTransactionsCommandHandler(AppDbContext context, ISender mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task Handle(CheckForReoccuringTransactionsCommand request, CancellationToken cancellationToken)
        {
            var reoccuringTransactions = await _mediator.Send(new GetTransactionsQuery(){
                UserId = request.UserId,
                IsReoccuring = true
            });
            
            if(reoccuringTransactions.Count == 0){
                return;
            }

            foreach(var transaction in reoccuringTransactions){
                if(transaction.NextReoccuranceDate != null && transaction.NextReoccuranceDate <= DateTime.Today){

                    while(transaction.NextReoccuranceDate <= DateTime.Today){
                        var newTransaction = new Transaction(){
                            Amount = transaction.Amount,
                            AccountId = transaction.AccountId,
                            Account = transaction.Account,
                            CategoryId = transaction.CategoryId,
                            Category = transaction.Category,
                            Description = transaction.Description,
                            Date = (DateTime)transaction.NextReoccuranceDate,
                            IsReoccuring = false,
                            ReoccuranceFrequency = null,
                            NextReoccuranceDate = null,
                            TransactionTags = transaction.TransactionTags
                        };

                        switch(transaction.ReoccuranceFrequency){
                            case ReoccuranceFrequency.Daily:
                                transaction.NextReoccuranceDate = transaction.NextReoccuranceDate!.Value.AddDays(1);
                                break;
                            case ReoccuranceFrequency.Weekly:
                                transaction.NextReoccuranceDate = transaction.NextReoccuranceDate!.Value.AddDays(7);
                                break;
                            case ReoccuranceFrequency.Monthly:
                                transaction.NextReoccuranceDate = transaction.NextReoccuranceDate!.Value.AddMonths(1);
                                break;
                            case ReoccuranceFrequency.Yearly:
                                transaction.NextReoccuranceDate = transaction.NextReoccuranceDate!.Value.AddYears(1);
                                break;
                        }

                        _context.Transactions.Add(newTransaction);
                    }
                }
            }
        }
    }
}