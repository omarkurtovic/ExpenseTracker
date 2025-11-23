# Expense Tracker - AI Coding Instructions

## Project Overview
**Expense Tracker** is a personal finance management application built with Blazor Server, featuring transaction tracking, account management, category organization, recurring transactions, tag support, and dashboard analytics with dark mode.

**Tech Stack**: Blazor Server, EF Core 9.0 + SQLite, ASP.NET Identity, MudBlazor 8.14.0, ApexCharts, xUnit 2.9.2, CsvHelper 33.1.0

**Key Projects**: 
- `ExpenseTrackerWebApp` - Main Blazor Server application
- `ExpenseTrackerTests` - xUnit tests with in-memory SQLite

---

## Architecture Patterns

### Multi-Tenant Data Isolation
Every entity (Account, Category, Transaction, Tag, UserPreferences) is scoped to `IdentityUserId`. Services enforce this via `ICurrentUserService.GetUserId()` to prevent data leaks between users.

**Critical Pattern**: All service queries filter by `IdentityUserId` through helper methods:
```csharp
private IQueryable<Transaction> GetTransactionsQuery(AppDbContext context)
{
    return context.Transactions
        .Where(t => t.Account.IdentityUserId == _currentUserService.GetUserId());
}
```
This ensures EVERY query respects user boundaries. Always use these patterns when adding new query methods.

### Service Layer Structure
Services in `Services/` handle all business logic and data access:
- **Validation**: DTOs validated in `SaveAsync()` before persistence (null checks, empty strings)
- **Optimistic Save**: Early return on validation failure—no exceptions thrown
- **CRUD Updates**: Use `_context.Entry(oldEntity).CurrentValues.SetValues(newEntity)` for updates
- **Cascade Deletes**: Account deletion cascades to Transactions; Category deletion restricted to prevent orphans

**Key Services**:
- `AccountService` - Account CRUD, balance calculations (`InitialBalance + Sum(Transactions)`)
- `TransactionService` - Transaction CRUD, recurring transaction logic, tag associations
- `DashboardService` - Monthly aggregations, expense trends, category analytics
- `CategoryService` - Category CRUD with default colors/icons
- `TagService` - Tag management and transaction-tag associations

### DTO Pattern
DTOs (`Dtos/`) separate API contracts from database models. Services convert between layers.

**Examples**: `AccountDto` (CRUD), `AccountWithBalance` (read with computed balance), `TransactionDto` (includes `Reoccuring`, `ReoccuranceFrequency`), `DashboardSummary` (aggregated data).

### Razor Component Architecture
Page components (`Components/Pages/`) + Dialog components (`Components/Pages/Dialogs/`):

**Page Flow**:
1. `OnInitializedAsync()` → `UpdatePage()` → Load data from service
2. Show loading spinner (`MudProgressCircular Indeterminate="true"`)
3. Render cards/lists with Edit/Delete buttons
4. Dialog submit → `await UpdatePage()` to refresh UI

**Dialog Flow** (see `AccountDialog.razor` as template):
1. `[Parameter] int? EntityId` - null = add, value = edit
2. `OnParametersSetAsync()` → Load entity if editing, initialize DTO
3. `MudForm` for validation before submit
4. `MudDialog.Close(DialogResult.Ok())` on success, triggering parent refresh

**Key Dialog Config**:
```csharp
var options = new DialogOptions 
{ 
    CloseOnEscapeKey = true, 
    CloseButton = true, 
    MaxWidth = MaxWidth.Medium 
};
```

---

## Critical Developer Workflows

### Building & Running
```powershell
# Restore & build solution
dotnet build ExpenseTracker.sln

# Run app with auto-recompile (Blazor Server dev workflow)
dotnet watch run --project ExpenseTrackerWebApp

# Run all tests
dotnet test

# Run specific test file
dotnet test ExpenseTrackerTests/Tests/AccountServiceTests.cs

# Docker deployment
docker-compose up
```

### Database Migrations
Migrations in `Migrations/` auto-run on app startup via `db.Database.Migrate()` in `Program.cs`. After modifying `AppDbContext.cs` or model files:
```powershell
dotnet ef migrations add DescriptiveName --project ExpenseTrackerWebApp
```
Migrations immediately take effect on next app run. Test migrations with `dotnet test` to ensure seeding works.

### Testing Approach
Tests use **in-memory SQLite** (disposable connection per test class). Test setup pattern:
1. Constructor: Create SqliteConnection + DbContextOptions
2. `context.Database.EnsureCreated()` - Apply schema without migrations
3. Seed test users, accounts, transactions
4. Inject `TestCurrentUserService` (mock) to simulate logged-in users
5. Assert data isolation per user

**Multi-user test example**: Create Alice, Bob, Charlie with different data; verify Bob can't see Alice's transactions.

Key test files: `AccountServiceTests.cs`, `DashboardServiceTests.cs`, `RecurringTransactionsTests.cs`, `TagServiceTests.cs`.

---

## Project-Specific Conventions

### Naming & File Organization
- **Services**: `{Entity}Service.cs` handles all CRUD for that entity (e.g., `AccountService.cs`, `TransactionService.cs`)
- **Models**: Entity models in `Database/Models/` (e.g., `Transaction.cs`, `Account.cs`) mirror database schema
- **Razor pages**: Top-level pages in `Components/Pages/` (e.g., `Accounts.razor`), dialogs in `Components/Pages/Dialogs/` (e.g., `AccountDialog.razor`)
- **DTOs**: Mirror model names with "Dto" suffix (e.g., `TransactionDto.cs`)
- **Recurring transactions**: Model uses `IsReoccuring`, `ReoccuranceFrequency`, `NextReoccuranceDate` properties

### Validation Rules
- Services validate required fields before saving: non-empty strings, non-null amounts, valid CategoryId/AccountId
- `SaveAsync()` returns early (no-op) if validation fails—**no exceptions thrown**
- Database constraints: Accounts/Categories have cascade delete on user deletion; Transaction→Category uses `Restrict` to prevent orphaned transactions
- Category deletion is restricted; reassign transactions first

### Date & Time Handling
- All dates use `DateTime.Now` (local machine time)
- Dashboard calculations use `DateTime.StartOfMonth()` extension (from MudBlazor)
- Month grouping: `GroupBy(t => new { t.Date.Year, t.Date.Month })`
- Culture set to `en-US` in `Program.cs` for consistent formatting
- Recurring transactions: `NextReoccuranceDate = Date.AddDays(7)` for weekly, `AddDays(1)` for daily, `AddMonths(1)` for monthly

### Decimal Precision
All money fields use `decimal` with `Precision(18, 2)`:
```csharp
modelBuilder.Entity<Transaction>()
    .Property(t => t.Amount)
    .HasPrecision(18, 2);
```

### UI/UX Patterns (MudBlazor 8.14.0)
- **Cards**: Transaction/account cards use `MudCard Outlined="true" Class="pa-2 border-2"`
- **Dialogs**: `CloseOnEscapeKey=true`, `CloseButton=true`, `MaxWidth=Medium`
- **Snackbars**: Show success/error feedback; positioned bottom-left (configured in `Program.cs`)
- **Loading states**: Full-page spinners while fetching data (`MudProgressCircular Indeterminate="true"`)
- **Icons**: Use MudBlazor Material icons; stored as strings (e.g., `Icons.Material.Filled.Payments`)

### Delete Behavior
- Deleting an account cascades to delete all related transactions
- Deleting a category is restricted (must reassign transactions first via `OnDelete(DeleteBehavior.Restrict)`)
- Confirm dialogs (`ConfirmDialog`) always prompt before destructive operations

---

## Cross-Component Communication

### Service Injection & Data Flow
Pages inject services, call them in `OnInitializedAsync()`, then pass data to child components via `[Parameter]`.

```
Page (Accounts.razor) 
  → AccountService.GetAllWithBalanceAsync() 
  → Display cards + Edit/Delete handlers 
  → Dialog (AccountDialog.razor) 
  → AccountService.SaveAsync() 
  → Snackbar.Add() + await UpdatePage()
```

### State Management
- No global state; services are scoped
- Pages reload data after CRUD operations (`await UpdatePage()` in parent)
- Dialogs return `DialogResult.Ok()` to signal success, parent refreshes
- `CascadingAuthenticationState` from `Program.cs` provides auth state to all components

---

## External Dependencies & Integrations

- **MudBlazor 8.14.0**: UI components, dialogs, snackbars, forms, icon library
- **ApexCharts**: Line/pie charts on dashboard (light theme configured in `Program.cs`)
- **CsvHelper 33.1.0**: Transaction CSV export (referenced in `ExportDialog.razor`)
- **EF Core 9.0.0**: SQLite provider, design tools (`dotnet ef` commands)
- **ASP.NET Identity**: User authentication, roles (seeded demo user "sa" with admin role)
- **xUnit 2.9.2**: Test framework with `Fact` and `Theory` attributes

---

## Key Files Reference

| File | Purpose |
|------|---------|
| `Program.cs` | DI setup, middleware, database initialization, ApexCharts/MudBlazor config |
| `AppDbContext.cs` | EF Core models, relationships, migration metadata, seed data |
| `Services/AccountService.cs` | Account CRUD with user isolation, balance calculations |
| `Services/TransactionService.cs` | Transaction CRUD, recurring transaction logic (`IsReoccuring`, `ReoccuranceFrequency`, `NextReoccuranceDate`) |
| `Services/DashboardService.cs` | Monthly aggregations, category analytics, expense trends |
| `Services/CurrentUserService.cs` | Claims-based user ID extraction from HTTP context |
| `Components/Pages/Accounts.razor` | Account listing, add/edit/delete UI with `UpdatePage()` pattern |
| `Components/Pages/Dialogs/AccountDialog.razor` | Standard dialog template for CRUD (model for other dialogs) |
| `Database/Models/Transaction.cs` | Transaction entity with `ReoccuranceFrequency` enum |
| `Dtos/TransactionDto.cs` | Includes `Reoccuring`, `ReoccuranceFrequency` for API contracts |
| `Tests/AccountServiceTests.cs` | Multi-user test scenarios with in-memory SQLite setup |
| `Tests/RecurringTransactionsTests.cs` | Recurring transaction logic validation |

---

## Common Pitfalls & Solutions

| Issue | Solution |
|-------|----------|
| Data visible across users | Always filter queries by `IdentityUserId` via `GetAccountsQuery()` pattern |
| Decimal precision loss | Use `Precision(18, 2)` in model builder for all money fields |
| Stale UI after CRUD | Call `await UpdatePage()` in parent after dialog closes |
| Transaction cascade delete | Account deletion cascades; category deletion is restricted |
| Typo in recurring property | Property is `IsReoccuring` (not `IsRecurring`), `ReoccuranceFrequency`, `NextReoccuranceDate` |
| Tests fail with schema mismatch | Seed test data matches migrations; use `EnsureCreated()` before seeding |
| Queries loading too much data | Use `.Include()` selectively; dashboard & transaction queries need optimization for 1000+ transactions |

---

## Before Implementing Features

1. **User Isolation**: Does the new feature respect user boundaries?
2. **DTOs**: Do you need a DTO, or can services work directly with models?
3. **Dialog UI**: Are you adding CRUD? Use MudBlazor dialog pattern (see `AccountDialog.razor`)
4. **Validation**: What required fields must you check? Add to service `SaveAsync()` method
5. **Tests**: Multi-user scenarios? Write tests with different user IDs in `ExpenseTrackerTests/Tests/`
6. **Database**: New entities? Add to `AppDbContext`, create migration, seed test data

---

## Performance Notes

From `todo.txt`: Dashboard & transaction queries need optimization for large datasets. Mobile responsiveness incomplete. Test with 1000+ transactions to identify bottlenecks (sorting, filtering, aggregation).
