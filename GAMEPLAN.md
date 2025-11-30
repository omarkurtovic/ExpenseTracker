# Expense Tracker - Development Roadmap

## Current Status
- why is form input so slow?

## Known Issues to Fix

- [ ] Enter key to submit forms (use Tab+Click instead)
- [ ] Dashboard query optimization (1000+ transactions)
-- opening existing tracsaction that is expense the amount should not be negative

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

Budgets (11 hours) - Real feature, domain logic, impressive
Performance optimization (4-6 hours) - Shows you can think about scale
Transfers (3.5 hours) - Quick win, feels complete
Background jobs for recurring (8 hours) - Shows scheduler knowledge