namespace ExpenseTrackerWasmWebApp.Features.SharedKernel.Services
{
    public class LayoutStateService
    {
        private bool _drawerOpen = true;

        public bool DrawerOpen
        {
            get => _drawerOpen;
            set
            {
                _drawerOpen = value;
                NotifyStateChanged();
            }
        }

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void ToggleDrawer()
        {
            DrawerOpen = !DrawerOpen;
        }
    }
}
