# Expense Tracker - Development Roadmap

## Current Status
- ✅ Live on Azure
- ✅ Mobile responsive
- ✅ Core features: Transactions, Accounts, Categories, Tags, Dashboard
- ✅ Multi-tenant architecture with user isolation
- ⚠️ Some UI bugs and polish needed

---

## Known Issues to Fix

From `todo.txt`:
- [ ] Tab highlighting in Transactions/Recurring (styling broken)
- [ ] Icon picker sizing in account dialog
- [ ] Enter key to submit forms (use Tab+Click instead)
- [ ] Mobile responsiveness gaps
- [ ] Dashboard query optimization (1000+ transactions)

---

## Potential Features

### Quick Wins (3-5 hours)
- **Transfer between accounts** - Move money between your own accounts
- **Import transactions** - CSV/bank data import with categorization
- **Better date range filtering** - Filter transactions by custom date ranges

### Medium Features (1-2 weeks)
- **Budget system** - Set budgets per category, track spending with alerts
- **Better recurring transactions** - UI to pause/edit/view recurring txns
- **Advanced dashboard** - Trends, month-over-month comparisons, forecasts
- **Transaction search** - Full-text search on descriptions, tags

### Larger Features
- **Background jobs** - Auto-run recurring transactions on schedule (not just on login)
- **Email reports** - Monthly spending digest emailed to user
- **Multi-currency** - Support multiple currencies with conversion rates
- **Bill splitting** - Split transactions between multiple people

---

## General Architecture Notes

- All entities scoped to `IdentityUserId` for multi-tenant isolation
- Services handle business logic, validation happens in `SaveAsync()`
- DTOs separate API contracts from database models
- Razor dialogs use MudBlazor with `CloseOnEscapeKey=true`, `MaxWidth=Medium`
- In-memory SQLite for testing with realistic user scenarios

See `.github/copilot-instructions.md` for detailed patterns.
