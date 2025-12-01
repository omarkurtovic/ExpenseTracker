using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Categories.Dtos;
using MudBlazor;

namespace ExpenseTrackerTests.Features.Categories.Handlers
{
    public class CategoryWithStatsDtoTests
    {
        [Fact]
        public void ComparisonColor_ExpenseWithLowerCurrentMonth_ReturnsSuccess()
        {
            var dto = new CategoryWithStatsDto
            {
                Type = TransactionType.Expense,
                CurrentMonthAmount = 100m,
                LastMonthAmount = 150m
            };

            var color = dto.ComparisonColor;

            Assert.Equal(Color.Success, color);
        }

        [Fact]
        public void ComparisonColor_ExpenseWithHigherCurrentMonth_ReturnsError()
        {
            var dto = new CategoryWithStatsDto
            {
                Type = TransactionType.Expense,
                CurrentMonthAmount = 200m,
                LastMonthAmount = 100m
            };

            var color = dto.ComparisonColor;

            Assert.Equal(Color.Error, color);
        }

        [Fact]
        public void ComparisonColor_ExpenseWithEqualMonths_ReturnsDefault()
        {
            var dto = new CategoryWithStatsDto
            {
                Type = TransactionType.Expense,
                CurrentMonthAmount = 150m,
                LastMonthAmount = 150m
            };

            var color = dto.ComparisonColor;

            Assert.Equal(Color.Default, color);
        }

        [Fact]
        public void ComparisonColor_IncomeWithHigherCurrentMonth_ReturnsSuccess()
        {
            var dto = new CategoryWithStatsDto
            {
                Type = TransactionType.Income,
                CurrentMonthAmount = 3500m,
                LastMonthAmount = 3000m
            };

            var color = dto.ComparisonColor;

            Assert.Equal(Color.Success, color);
        }

        [Fact]
        public void ComparisonColor_IncomeWithLowerCurrentMonth_ReturnsError()
        {
            var dto = new CategoryWithStatsDto
            {
                Type = TransactionType.Income,
                CurrentMonthAmount = 2500m,
                LastMonthAmount = 3000m
            };

            var color = dto.ComparisonColor;

            Assert.Equal(Color.Error, color);
        }

        [Fact]
        public void ComparisonColor_IncomeWithEqualMonths_ReturnsDefault()
        {
            var dto = new CategoryWithStatsDto
            {
                Type = TransactionType.Income,
                CurrentMonthAmount = 3000m,
                LastMonthAmount = 3000m
            };

            var color = dto.ComparisonColor;

            Assert.Equal(Color.Default, color);
        }

        [Fact]
        public void ComparisonColor_ZeroLastMonthAmount_ReturnsDefault()
        {
            var dto = new CategoryWithStatsDto
            {
                Type = TransactionType.Expense,
                CurrentMonthAmount = 100m,
                LastMonthAmount = 0m
            };

            var color = dto.ComparisonColor;

            Assert.Equal(Color.Default, color);
        }

        [Fact]
        public void ComparisonDisplay_ExpenseWithPercentageIncrease_ShowsUpArrowAndPercentage()
        {
            var dto = new CategoryWithStatsDto
            {
                Type = TransactionType.Expense,
                CurrentMonthAmount = 200m,
                LastMonthAmount = 100m
            };

            var display = dto.ComparisonDisplay;

            Assert.Contains("↑", display);
            Assert.Contains("100", display); // 100% increase
            Assert.Contains("%", display);
        }

        [Fact]
        public void ComparisonDisplay_ExpenseWithPercentageDecrease_ShowsDownArrowAndPercentage()
        {
            var dto = new CategoryWithStatsDto
            {
                Type = TransactionType.Expense,
                CurrentMonthAmount = 100m,
                LastMonthAmount = 200m
            };

            var display = dto.ComparisonDisplay;

            Assert.Contains("↓", display);
            Assert.Contains("50", display); // 50% decrease
            Assert.Contains("%", display);
        }

        [Fact]
        public void ComparisonDisplay_ZeroLastMonthAmount_ShowsDash()
        {
            var dto = new CategoryWithStatsDto
            {
                Type = TransactionType.Expense,
                CurrentMonthAmount = 100m,
                LastMonthAmount = 0m
            };

            var display = dto.ComparisonDisplay;

            Assert.Equal("—", display);
        }

        [Fact]
        public void ComparisonDisplay_EqualMonths_ShowsDashArrowAndZeroPercentage()
        {
            var dto = new CategoryWithStatsDto
            {
                Type = TransactionType.Expense,
                CurrentMonthAmount = 100m,
                LastMonthAmount = 100m
            };

            var display = dto.ComparisonDisplay;

            Assert.Contains("—", display);
            Assert.Contains("0", display); // 0% change
            Assert.Contains("%", display);
        }

        [Fact]
        public void ComparisonDisplay_IncomeDecreaseToZero_HandlesProperly()
        {
            var dto = new CategoryWithStatsDto
            {
                Type = TransactionType.Income,
                CurrentMonthAmount = 0m,
                LastMonthAmount = 1000m
            };

            var display = dto.ComparisonDisplay;

            Assert.Contains("↓", display);
            Assert.Contains("100", display); // 100% decrease
            Assert.Contains("%", display);
        }

        [Fact]
        public void ComparisonDisplay_SmallPercentageChange_RoundsCorrectly()
        {
            var dto = new CategoryWithStatsDto
            {
                Type = TransactionType.Expense,
                CurrentMonthAmount = 101m,
                LastMonthAmount = 100m
            };

            var display = dto.ComparisonDisplay;

            Assert.Contains("↑", display);
            Assert.Contains("1", display); // ~1% increase
            Assert.Contains("%", display);
        }

        [Fact]
        public void ComparisonDisplay_LargePercentageChange_CalculatesCorrectly()
        {
            var dto = new CategoryWithStatsDto
            {
                Type = TransactionType.Expense,
                CurrentMonthAmount = 500m,
                LastMonthAmount = 100m
            };

            var display = dto.ComparisonDisplay;

            Assert.Contains("↑", display);
            Assert.Contains("400", display); // 400% increase
            Assert.Contains("%", display);
        }
    }
}
