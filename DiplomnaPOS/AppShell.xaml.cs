using DiplomnaPOS.Pages;
using DiplomnaPOS.Pages.AuthenticationPages;

namespace DiplomnaPOS
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(StartPage), typeof(StartPage));

            Routing.RegisterRoute("DashboardPage", typeof(DashboardPage));
            Routing.RegisterRoute("DeviceManagerPage", typeof(DeviceManagerPage));
            Routing.RegisterRoute("SettingsPage", typeof(SettingsPage));

            Routing.RegisterRoute("UpdateMenuPage", typeof(UpdateMenuPage));
            Routing.RegisterRoute("RefundsPage", typeof(RefundsPage));
            Routing.RegisterRoute("ReceiptManagerPage", typeof(ReceiptManagerPage));
            Routing.RegisterRoute("EmployeeManagementPage", typeof(EmployeeManagementPage));
        }
    }
}
