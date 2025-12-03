using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Services
{
    public class TagService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public TagService(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        private IQueryable<Tag> GetTagsQuery(AppDbContext context)
        {
            return context.Tags.Where(t => t.IdentityUserId == _currentUserService.GetUserId());
        }

        public async Task<List<Tag>> GetAllAsync()
        {
            return await GetTagsQuery(_context).OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<Tag?> GetAsync(int tagId)
        {
            return await GetTagsQuery(_context).SingleOrDefaultAsync(t => t.Id == tagId);
        }

        public async Task<Tag?> GetByNameAsync(string name)
        {
            return await GetTagsQuery(_context).SingleOrDefaultAsync(t => t.Name == name);
        }

        public async Task<Tag> SaveAsync(string name, string? color = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Tag name cannot be empty");
            }

            var userId = _currentUserService.GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User not authenticated");
            }

            var existingTag = await GetByNameAsync(name);
            if (existingTag != null)
            {
                return existingTag;
            }

            var tag = new Tag
            {
                Name = name,
                Color = color ?? "#9E9E9E", 
                IdentityUserId = userId
            };

            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task DeleteAsync(int tagId)
        {
            await _context.Tags.Where(t => t.Id == tagId).ExecuteDeleteAsync();
            _context.ChangeTracker.Clear();
        }


        public async Task SetTransactionTagsAsync(int transactionId, List<int> tagIds)
        {
            await _context.TransactionTags
                .Where(tt => tt.TransactionId == transactionId)
                .ExecuteDeleteAsync();

            _context.ChangeTracker.Clear();

            if (tagIds.Count == 0)
                return;

            var newTransactionTags = tagIds
                .Select(tagId => new TransactionTag { TransactionId = transactionId, TagId = tagId })
                .ToList();

            await _context.TransactionTags.AddRangeAsync(newTransactionTags);
            await _context.SaveChangesAsync();
        }
    }
}
