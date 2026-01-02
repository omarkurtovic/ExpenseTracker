
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Auth.Commands;
using ExpenseTrackerWebApi.Features.Tags.Models;
using ExpenseTrackerWebApi.Features.Transactions.Models;
using ExpenseTrackerWebApi.Features.Transactions.Queries;
using MediatR;

namespace  ExpenseTrackerWebApi.Features.Auth.Handlers
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
            var reoccuringTransactions = await _mediator.Send(new GetAllTransactionsQuery(){
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
                            Amount = (decimal)transaction.Amount!,
                            AccountId = (int)transaction.AccountId!,
                            CategoryId = (int)transaction.CategoryId!,
                            Description = transaction.Description,
                            Date = (DateTime)transaction.NextReoccuranceDate,
                            IsReoccuring = false,
                            ReoccuranceFrequency = null,
                            NextReoccuranceDate = null
                        };

                        switch(transaction.ReoccuranceFrequency){
                            case ReoccuranceFrequencyDto.Daily:
                                transaction.NextReoccuranceDate = transaction.NextReoccuranceDate!.Value.AddDays(1);
                                break;
                            case ReoccuranceFrequencyDto.Weekly:
                                transaction.NextReoccuranceDate = transaction.NextReoccuranceDate!.Value.AddDays(7);
                                break;
                            case ReoccuranceFrequencyDto.Monthly:
                                transaction.NextReoccuranceDate = transaction.NextReoccuranceDate!.Value.AddMonths(1);
                                break;
                            case ReoccuranceFrequencyDto.Yearly:
                                transaction.NextReoccuranceDate = transaction.NextReoccuranceDate!.Value.AddYears(1);
                                break;
                        }

                        _context.Transactions.Add(newTransaction);
                        await _context.SaveChangesAsync();
                        var newTags = new List<TransactionTag>();
                        foreach(var tag in transaction.TransactionTags){
                            var newTag = new TransactionTag(){
                                TransactionId = newTransaction.Id,
                                TagId = tag.TagId
                            };
                            newTags.Add(newTag);
                        }
                        _context.TransactionTags.AddRange(newTags);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}