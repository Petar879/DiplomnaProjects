using DiplomnaPOS.Services;
using MauiIcons.Core;
using MauiIcons.Fluent;
using QuestPDF;

namespace DiplomnaPOS.Pages.AuthenticationPages;

public partial class LoadingPage : ContentPage
{
    private readonly AuthServices _authService;
    private bool _isAdmin = false;
    public LoadingPage(AuthServices authService)
	{
		InitializeComponent();
        _authService = authService;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (await _authService.IsAuthenticatedAsync())
        {
            // User is logged in
            // redirect to main page

            if (await SecureStorage.Default.GetAsync("loggedUserRole") == "Admin")
            {
                AppShell.Current.Items.Where(shellItem => shellItem.Title == "Settings").FirstOrDefault().FlyoutItemIsVisible = true;
            }
            else
            {
                AppShell.Current.Items.Where(shellItem => shellItem.Title == "Settings").FirstOrDefault().FlyoutItemIsVisible = false;
            }

            await Shell.Current.GoToAsync("//StartPage");
            //await Shell.Current.GoToAsync($"//{nameof(StartPage)}");

        }
        else
        {
            
            // User is not logged in
            // Redirect to LoadingPage
            //await DisplayAlert("sds", "sdsd", "dsdss");

            //await Shell.Current.GoToAsync("//LoginPage");

            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");

            //await Shell.Current.GoToAsync("LoginPage");

        }
    }
}