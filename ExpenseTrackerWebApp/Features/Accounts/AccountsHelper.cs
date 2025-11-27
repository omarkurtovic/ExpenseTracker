using MudBlazor;

namespace ExpenseTrackerWebApp.Features.Accounts{
    public class AccountsHelper
    {
        public static List<string> GetDefaultAccountIcons()
        {
            return new List<string>
            {
                Icons.Material.Filled.Payments,
                Icons.Material.Filled.AccountBalance,
                Icons.Material.Filled.Wallet,
                Icons.Material.Filled.CreditCard,
                Icons.Material.Filled.Savings,
                Icons.Material.Filled.AttachMoney,
                Icons.Material.Filled.StoreMallDirectory,
                Icons.Material.Filled.LocalAtm,
                Icons.Material.Filled.MonetizationOn,
                Icons.Material.Filled.PointOfSale,
                Icons.Material.Filled.ShoppingCart,
                Icons.Material.Filled.AccountBalanceWallet
            };
        }
    }
}