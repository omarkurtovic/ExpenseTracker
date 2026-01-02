using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Tags.Commands;
using ExpenseTrackerWebApi.Features.Tags.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApi.Features.Tags.Handlers
{
    public class ResetToDefaultTagsCommandHandler : IRequestHandler<ResetToDefaultTagsCommand>
    {
        private readonly AppDbContext _context;

        public ResetToDefaultTagsCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(ResetToDefaultTagsCommand request, CancellationToken cancellationToken)
        {
            await _context.Tags.Where(c => c.IdentityUserId == request.UserId)
                .ExecuteDeleteAsync(cancellationToken);
            
            _context.ChangeTracker.Clear();

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
                var tag = new Tag(){Name = name, Color = color, IdentityUserId = request.UserId};
                tags.Add(tag);
            }
            _context.Tags.AddRange(tags);
            await _context.SaveChangesAsync();
        }
    }
}