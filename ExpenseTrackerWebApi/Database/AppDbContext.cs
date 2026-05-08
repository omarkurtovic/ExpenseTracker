using ExpenseTrackerWebApi.Features.Accounts.Models;
using ExpenseTrackerWebApi.Features.Budgets.Models;
using ExpenseTrackerWebApi.Features.Categories.Models;
using ExpenseTrackerWebApi.Features.Logging.Models;
using ExpenseTrackerWebApi.Features.Tags.Models;
using ExpenseTrackerWebApi.Features.Transactions.Models;
using ExpenseTrackerWebApi.Features.UserPreferences.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApi.Database
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<TransactionTag> TransactionTags { get; set; }
        public virtual DbSet<UserPreference> UserPreferences { get; set; }
        public virtual DbSet<Budget> Budgets { get; set; }
        public virtual DbSet<BudgetAccount> BudgetAccounts { get; set; }
        public virtual DbSet<BudgetCategory> BudgetCategories { get; set; }
        public virtual DbSet<SystemLog> SystemLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserPreference>()
             .HasKey(up => up.UserId);

            modelBuilder.Entity<UserPreference>()
               .HasOne(up => up.IdentityUser)
               .WithOne()
               .HasForeignKey<UserPreference>(up => up.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.IdentityUser)
                .WithMany()
                .HasForeignKey(a => a.IdentityUserId)
                .IsRequired();

            modelBuilder.Entity<Category>()
                .HasOne(a => a.IdentityUser)
                .WithMany()
                .HasForeignKey(a => a.IdentityUserId)
                .IsRequired();

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tag>()
                .HasOne(t => t.IdentityUser)
                .WithMany()
                .HasForeignKey(t => t.IdentityUserId)
                .IsRequired();

            modelBuilder.Entity<TransactionTag>()
                .HasKey(tt => new { tt.TransactionId, tt.TagId });

            modelBuilder.Entity<TransactionTag>()
                .HasOne(tt => tt.Transaction)
                .WithMany(t => t.TransactionTags)
                .HasForeignKey(tt => tt.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TransactionTag>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.TransactionTags)
                .HasForeignKey(tt => tt.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SystemLog>()
                .HasOne(a => a.IdentityUser)
                .WithMany()
                .HasForeignKey(a => a.IdentityUserId)
                .IsRequired();

            // budget
            modelBuilder.Entity<Budget>()
                .HasOne(b => b.IdentityUser)
                .WithMany()
                .HasForeignKey(b => b.IdentityUserId)
                .IsRequired();


            modelBuilder.Entity<BudgetCategory>()
                .HasKey(bc => new { bc.BudgetId, bc.CategoryId });

            modelBuilder.Entity<BudgetAccount>()
                .HasKey(ba => new { ba.BudgetId, ba.AccountId });

            modelBuilder.Entity<BudgetCategory>()
                .HasOne(bc => bc.Budget)
                .WithMany(bc => bc.BudgetCategories)
                .HasForeignKey(bc => bc.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BudgetAccount>()
                .HasOne(ac => ac.Budget)
                .WithMany(ac => ac.BudgetAccounts)
                .HasForeignKey(ac => ac.BudgetId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Account>()
                .Property(t => t.InitialBalance)
                .HasPrecision(18, 2);

        }
    }
}