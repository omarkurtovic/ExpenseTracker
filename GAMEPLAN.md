# Expense Tracker - Development Gameplan

---

## 🚀 UPDATED: ML-Focused Direction (4-Week Plan)

**As of now**: You're shifting focus from polish/features to a **high-impact learning goal**.

### Why ML Over Other Features?
- **Rarity**: Most juniors never touch production ML—huge differentiator
- **Salary**: ML skills command 15-30% higher salary
- **Problem**: Users spend 10+ minutes/week manually categorizing (real pain)
- **Timeline**: 4 weeks is realistic (Week 1: data, Week 2: training, Week 3: integration, Week 4: deploy)
- **Foundation**: You have Andrew Ng course + Python = ready to learn ML.NET

### 4-Week ML Roadmap Summary

| Week | Focus | Deliverable |
|------|-------|-------------|
| 1 | Data extraction & preparation | CSV with 100+ transactions |
| 2 | Model building & training | Trained model with 80%+ accuracy |
| 3 | Integration & feedback loop | Suggestions in UI, feedback logged |
| 4 | Deploy & blog | Live on Azure + published blog post |

**See `ML_ROADMAP.md` for full day-by-day breakdown.**

### Key Decision
- **Polish features** (budgets, recurring txns, transfers) = adding checkboxes to portfolio
- **ML categorization** = learning a valuable, rare skill that directly impacts salary

**Recommendation**: Go all-in on ML for 4 weeks. It's better long-term ROI.

---

## 📊 Where You Are Now
- **Core Features**: ✅ Transactions, Accounts, Categories, Tags, Dark Mode, Recurring transactions (basic), Dashboard
- **Backend**: ✅ Strong architecture with user isolation, DTOs, proper service layer
- **Auth**: ✅ Registration/Login with ASP.NET Identity
- **Testing**: ✅ Multi-user tests with in-memory SQLite
- **UI**: ✅ MudBlazor foundation with cards, dialogs, snackbars
- **Recurring Txns**: ⚠️ Basic—only `CheckForReoccuringTransactions()` on login, limited control

---

## 🎯 Realistic Priority Ladder
*Ordered by: impact on UX × portfolio value × implementation effort*

### **PHASE 1: Quick Wins** (1-2 weeks, high visual impact)
These make the biggest impression on portfolio reviewers with minimal effort.

#### 1.1 **Fix Existing Issues from Todo** (HIGH PRIORITY)
   - ✅ Mobile responsiveness: Add responsive grid classes to all pages
     - Currently: `md="6"` on dashboard—need to handle `xs="12"`  
     - Impact: Portfolio killer if mobile looks broken
     - Work: 4-6 hours across all pages
   
   - ✅ Transaction/Recurring tabs not highlighted
     - Currently: Two tabs but styling broken (seen in todo)
     - Work: 1-2 hours in `Transactions.razor`
   
   - ✅ Icon picker size issue in account dialog
     - Currently: Icon selection dropdown messed up
     - Work: 30 min CSS fix

   - ✅ "On Enter to submit data"
     - Allow Enter key to submit forms instead of Tab+Click
     - Work: 1 hour across dialogs

   **Why First**: These are broken experiences that make you look sloppy. Reviewers notice.

---

#### 1.2 **Polish Dashboard** (HIGH PORTFOLIO VALUE)
   - Add **Date Range Picker** (portfolio feature)
     - Current: Hard-coded "last 6 months"
     - Add: `DateRange` selection on dashboard
     - Shows: You can handle complex date logic
     - Work: 3-4 hours (use MudBlazor's `DateRangePicker`)
   
   - Add **Export to PDF** (alongside CSV)
     - Current: CSV export only
     - Add: Dashboard snapshot as PDF (title, filters, charts)
     - Library: `QuestPDF` or `iText` (~2 hours)
     - Portfolio: "Professional reporting features"
   
   - **Sparklines** for quick trends
     - Tiny inline charts next to each stat
     - Shows: Trend direction at a glance
     - Work: 1 hour with ApexCharts

   **Why Now**: Dashboard is the hero page. Impressive dashboard = impressive project.

---

#### 1.3 **Better Filtering & Search** (UX feels professional)
   - **Transaction Search**
     - Current: None—raw list
     - Add: Full-text search on Description + Tags
     - Add: Category multi-select filter
     - Add: Amount range slider (e.g., "$0-$100")
     - Work: 2-3 hours
   
   - **Date Range Picker** for transactions list
     - Not just dashboard—also in `Transactions.razor`
     - Work: 1 hour (copy from dashboard)

   **Why**: Search is a pro feature that makes the app feel "complete."

---

### **PHASE 2: Core Features** (2-3 weeks, portfolio-defining)
Pick ONE of these. They're bigger but unlock significant portfolio value.

#### 2.1 **Better Recurring Transaction Control** ⭐ (RECOMMENDED FIRST)
   **Current State**: 
   - Can mark transaction as recurring with frequency
   - Only runs on login via `CheckForReoccuringTransactions()`
   - No UI to manage recurrence (view/edit/pause/skip)
   
   **Add**:
   - ✅ **Recurring Transactions Dashboard**
     - View all active recurring txns
     - Pause/Resume button
     - Skip next occurrence
     - Edit recurrence (frequency/amount/date)
     - Delete recurrence (cancel future occurrences)
     - Work: 6-8 hours
   
   - ✅ **Auto-run on timer** (not just login)
     - Use background service (`IHostedService`)
     - Check every day at midnight for due recurrences
     - Work: 3-4 hours
   
   - ✅ **Recurrence History/Audit Log**
     - Show past auto-generated transactions
     - Mark which were auto-generated vs manual
     - Work: 2 hours (add `IsAutoGenerated` flag to Transaction)

   **Total**: ~12 hours  
   **Portfolio Value**: ⭐⭐⭐ (Shows understanding of scheduling, background jobs, audit trails)

---

#### 2.2 **Budget System** ⭐⭐ (HIGHEST PORTFOLIO VALUE)
   **Current State**: None
   
   **What to Build**:
   - Model: `Budget` (UserId, CategoryId, Amount, Period: Monthly/Weekly/Yearly)
   - Service: `BudgetService` (CRUD + progress calculation)
   - UI: 
     - Budget settings page (`/budgets`)
     - Budget dialog for add/edit
     - Dashboard widget: "Budget vs Actual" for each category
     - Color-coded progress bars (green <50%, yellow 50-100%, red >100%)
   - Alerts: 
     - Toast when spending hits 80% of budget
     - Dashboard warning if over budget
   
   **Work**: 
   - Model/Migration: 1 hour
   - Service: 2 hours
   - UI (settings page + dialogs): 4 hours
   - Dashboard integration: 2 hours
   - Tests: 2 hours
   - **Total: ~11 hours**

   **Portfolio Value**: ⭐⭐⭐⭐ (Budgets are a REAL app feature; shows you understand domain logic)

---

#### 2.3 **Transfer Between Accounts** ⭐ (Quick win, feels polished)
   **Current State**: None—only one-sided transactions
   
   **What to Build**:
   - New Dialog: `TransferDialog.razor`
     - From Account (dropdown)
     - To Account (dropdown)
     - Amount
   - Logic: Create TWO transactions (opposite amounts)
     - From account: negative amount to "Transfer Out" category
     - To account: positive amount to "Transfer In" category
   - UI: Button on Accounts page: "Transfer"
   
   **Work**: 
   - New category auto-creation: 30 min
   - Dialog + logic: 2 hours
   - Tests (multi-account transfers): 1 hour
   - **Total: ~3.5 hours**

   **Why**: Easy to implement, makes app feel more complete. Good "next feature" showcase.

---

### **PHASE 3: Polish & Performance** (1-2 weeks, quality signaling)
Do after Phase 1 & 2 basics are done.

#### 3.1 **Performance & Query Optimization** ⚠️
   **Current State**: Todo says "optimize queries for 1000+ transactions"
   
   **What to Do**:
   - Profile dashboard query (likely N+1 on categories)
   - Add `.Include()` strategically in `DashboardService`
   - Implement query pagination for transaction list
   - Add indexes to `Date`, `AccountId`, `CategoryId` in migration
   - Load test: Run with 1000+ transactions, measure query times
   
   **Work**: 4-6 hours  
   **Portfolio Value**: ⭐⭐ (Shows you understand performance; maybe not exciting to viewers)

---

#### 3.2 **Advanced Reporting** ⭐⭐ (Impressive, can be big)
   **Current State**: Basic dashboard charts
   
   **Add**:
   - **Monthly Trend Analysis**
     - Line chart: spending trend over time (6-12 months)
     - YoY comparison (this month vs. last year)
   
   - **Category Breakdown Insights**
     - Where's your money going? (pie chart)
     - Month-over-month change per category
     - Highlights biggest changes ("Food ↑ 23%")
   
   - **Cashflow Forecast** (stretch goal)
     - Predict balance based on recurring txns + average spending
     - Show "projected balance in 3 months"
   
   - **Reports Page** (`/reports`)
     - Selectable date ranges
     - Filterable by category/account
     - Export chart as image
   
   **Work**: 8-12 hours  
   **Portfolio Value**: ⭐⭐⭐ (Analytics is sexy; shows you can aggregate & visualize data)

---

#### 3.3 **Mobile Responsiveness Complete** ✅
   **Current State**: "Limited mobile responsiveness"
   
   **What to Do**:
   - Test on phone: Check every page
   - MudBlazor has `xs="12" md="6"` breakpoints—use them
   - Mobile-specific: Stack cards vertically, hide non-essential columns
   - Navigation: Add mobile drawer (burger menu) instead of top nav
   - Touch-friendly: Larger buttons/spacing on mobile
   
   **Work**: 6-8 hours  
   **Portfolio Value**: ⭐⭐ (Table stakes in 2025, but not exciting)

---

#### 3.4 **Better Auth & Validation** ⭐
   **Current State**: Basic registration/login page, demo user "sa"
   
   **Add**:
   - ✅ Email verification (send email with token link)
   - ✅ Password reset flow (forgot password link)
   - ✅ Real-time validation feedback (username exists? email format?)
   - ✅ Rate limiting on login (prevent brute force)
   - ✅ Remember me (optional for portfolio, but nice)
   
   **Work**: 6-8 hours  
   **Portfolio Value**: ⭐ (Security/auth is expected, less flashy)

---

#### 3.5 **Test Suite Expansion** ✅
   **Current State**: Basic multi-user tests, some services tested
   
   **Add**:
   - Tests for `DashboardService` (aggregation logic)
   - Tests for recurring transaction auto-run
   - Tests for Budget system (when added)
   - Integration tests: Full flow (login → create txn → view in dashboard)
   - Coverage goal: 70%+ for Services
   
   **Work**: 4-6 hours  
   **Portfolio Value**: ⭐ (Good engineering, but not visible to casual viewers)

---

### **PHASE 4: Fun/Stretch Goals** (nice to have)
Only if you want to continue after Phase 2.

#### 4.1 **Multi-Currency Support**
   - Add `CurrencyCode` to Account
   - Show conversion rates in transfers
   - Work: 6-10 hours
   - Portfolio: ⭐ (Cool for global apps, but overkill for portfolio)

#### 4.2 **Dark Mode Enhancements**
   - Already have dark mode toggle
   - Could add: Auto dark mode based on system preference
   - Could add: Custom theme colors
   - Work: 2-3 hours
   - Portfolio: ⭐ (Nice, but not a deal-breaker)

#### 4.3 **Bill Splitting** (Social feature)
   - Split transactions between users
   - Track who owes whom
   - Mark as settled
   - Work: 8-12 hours (adds multi-user complexity)
   - Portfolio: ⭐⭐⭐ (Shows you can handle complex domain logic)

#### 4.4 **Notifications & Alerts**
   - Email digest: Monthly summary
   - In-app alerts for budget overages
   - Notification preferences
   - Work: 4-6 hours
   - Portfolio: ⭐ (Polish feature)

---

## 🚀 My Recommended Path (for Portfolio + Learning)

### **Week 1-2: Phase 1 Polish**
```
Day 1-2:   Fix mobile responsiveness + existing bugs (tab highlighting, icon picker, Enter key)
Day 3-4:   Dashboard improvements (date range, export PDF, sparklines)
Day 5-7:   Transaction filtering & search
```
**Outcome**: App looks polished, works on mobile, feels professional.

---

### **Week 3-4: Phase 2a - Recurring Transactions** ⭐ RECOMMENDED
```
Day 1-3:   Build recurring dashboard page (view/pause/skip/edit)
Day 4-5:   Implement background service for auto-run
Day 6-7:   Tests + audit log
```
**Outcome**: Solid feature showing scheduling + background job knowledge. Plus: You can talk about it in interviews.

---

### **Week 5-6: Phase 2b - Budget System** ⭐⭐ (If you want more)
```
Day 1:     Model + Migration
Day 2-3:   BudgetService
Day 4-5:   UI (settings page, dialogs, dashboard widget)
Day 6-7:   Tests + alerts
```
**Outcome**: Real domain feature. Budgeting is something every user cares about. Huge portfolio differentiator.

---

### **Week 7+: Phase 3 - Pick Based on Mood**
- Performance + advanced reporting if you want to show data engineering skills
- Mobile + auth if you want to show polish
- Test suite if you're into TDD

---

## 📈 Why This Order?

| Feature | Portfolio Impact | Learning Value | Time | Why Now? |
|---------|-----------------|-----------------|------|----------|
| **Phase 1** | 🔴 Bug fixes | Low | 8h | Makes current app look professional |
| **Dashboard Polish** | 🟠 Visual | Medium | 4h | Reviewers see this first |
| **Filtering/Search** | 🟠 UX | Low | 3h | Shows attention to detail |
| **Recurring Txns** | 🟢 Feature | High | 12h | Scheduling + background jobs; interviewing gold |
| **Budgets** | 🟢🟢 Feature | High | 11h | Domain knowledge; shows you understand real apps |
| **Transfer Accounts** | 🟠 Completeness | Low | 3.5h | Easy win after budgets |
| **Performance** | 🟠 Engineering | Medium | 6h | Show you can optimize |
| **Advanced Reports** | 🟢 Visible | High | 12h | Beautiful, impressive; data aggregation skill |
| **Mobile** | 🟡 Table Stakes | Low | 8h | Expected in 2025 |
| **Auth/Validation** | 🟡 Security | Medium | 8h | Shows security thinking |

---

## 🎓 Learning Path Alignment

**If focused on**: 
- 🔵 **Backend/Databases** → Priority: Budgets → Reporting → Performance tuning
- 🟢 **Full-Stack** → Priority: Recurring Txns → Budgets → Mobile polish
- 🔴 **Frontend/UX** → Priority: Mobile → Dashboard → Reporting visualizations
- 🟡 **DevOps/Infrastructure** → Priority: Performance → Background jobs → Docker improvements

---

## ✅ Quick Decision Tree

**Pick your next feature by asking:**

1. **"Will this impress a recruiter in 2 minutes?"**
   - YES → Budget system (most impressive)
   - NO → Go to #2

2. **"Will this break the app for users?"**
   - YES → Phase 1 (fix bugs first)
   - NO → Go to #3

3. **"Do I understand the domain logic well enough?"**
   - YES → Recurring Txns or Budgets
   - NO → Go to #4

4. **"Am I tired of backend?"**
   - YES → Mobile responsiveness or Dashboard polish
   - NO → Recurring Txns or Budgets

---

## 💡 Pro Tips

- **Commit message discipline**: Use clear messages (recruiters read these)
- **Write ONE good test per feature**: Not 100, but at least one that covers happy path
- **Screenshot it**: When done, add to README with description
- **Blog post it**: Write up one feature (e.g., "How I Built Budget Tracking in Blazor") = portfolio gold
- **Don't skip Phase 1**: Professional apps start polished, not feature-complete

---

## 🔗 Git Workflow Suggestion
```
main
├── feature/phase-1-mobile-responsiveness
├── feature/phase-1-bug-fixes
├── feature/dashboard-date-range
├── feature/transaction-search
├── feature/recurring-transactions-ui
├── feature/budget-system
└── feature/transfer-accounts
```

Each feature in 1-3 commits with clear messages.

---

## Summary: What to Do **This Week**

```
1. Fix mobile (1-2 days)        → Makes app look professional
2. Fix bug list (½ day)          → Tab highlighting, icon picker, Enter key
3. Add date picker (½ day)       → Dashboard enhancement
4. Try recurring Txns (2 days)   → See if you enjoy scheduling logic

If yes to #4 → Commit to Phase 2a (recurring fully)
If no → Pick dashboard/search polish or budgets
```

Pick one. Don't overthink. Ship it. Learn from it.
