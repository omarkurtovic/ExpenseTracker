using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Categories.Commands;
using ExpenseTrackerWebApi.Features.Categories.Models;
using ExpenseTrackerWebApi.Features.Transactions.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace ExpenseTrackerWebApi.Features.Categories.Handlers
{
    public class ResetToDefaultCategoriesCommandHandler : IRequestHandler<ResetToDefaultCategoriesCommand>
    {
        private readonly AppDbContext _context;

        public ResetToDefaultCategoriesCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(ResetToDefaultCategoriesCommand request, CancellationToken cancellationToken)
        {
            await _context.Categories.Where(c => c.IdentityUserId == request.UserId)
                .ExecuteDeleteAsync(cancellationToken);
            
            _context.ChangeTracker.Clear();
            var categories = new List<Category>
            {
                new() {  Name = "Housing", Type=TransactionType.Expense, Icon=Icons.Material.Filled.Home, Color="#FF5733" },
                new() {  Name = "Transportation", Type=TransactionType.Expense, Icon=Icons.Material.Filled.DirectionsCar, Color="#33C1FF" },
                new() {  Name = "Groceries", Type=TransactionType.Expense, Icon=Icons.Material.Filled.LocalGroceryStore, Color="#33FF57" },
                new() {  Name = "Food & Dining", Type=TransactionType.Expense, Icon=Icons.Material.Filled.Restaurant, Color="#FF33A8" },
                new() {  Name = "Healthcare", Type=TransactionType.Expense, Icon=Icons.Material.Filled.HealthAndSafety, Color="#FF8C33" },
                new() {  Name = "Personal Care", Type=TransactionType.Expense, Icon=Icons.Material.Filled.Person, Color="#8C33FF" },
                new() {  Name = "Family Care", Type=TransactionType.Expense, Icon=Icons.Material.Filled.FamilyRestroom, Color="#33FFDD" },
                new() {  Name = "Debt Payments", Type=TransactionType.Expense, Icon=Icons.Material.Filled.CreditCard, Color="#FF3333" },
                new() {  Name = "Entertainment", Type=TransactionType.Expense, Icon=Icons.Material.Filled.Movie, Color="#33FF57" },
                new() {  Name = "Savings & Investing", Type=TransactionType.Expense, Icon=Icons.Material.Filled.Savings, Color="#3357FF" },
                new() {  Name = "Technology", Type=TransactionType.Expense, Icon=Icons.Material.Filled.Computer, Color="#FF33F6" },
                new() {  Name = "Other", Type=TransactionType.Expense, Icon=Icons.Material.Filled.Category, Color="#A9A9A9" },
                new() {  Name = "Income", Type=TransactionType.Income, Icon=Icons.Material.Filled.AttachMoney, Color="#33FF57" },
            };

            for(int i = 1; i <= categories.Count; i++)
            {
                categories[i - 1].IdentityUserId = request.UserId;
            }
            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();
        }
    }
}