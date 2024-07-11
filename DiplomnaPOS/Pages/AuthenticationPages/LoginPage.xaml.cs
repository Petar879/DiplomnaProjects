using DiplomnaPOS.Services;
using MauiIcons.Core;
using MauiIcons.Fluent;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiplomnaPOS.Pages.AuthenticationPages;

public partial class LoginPage : ContentPage
{
    private readonly AuthServices _authServices;

    public LoginPage(AuthServices authService)
	{
		InitializeComponent();
        _authServices = authService;
        LoginBtn.IsEnabled = false;

        emailValidator.PropertyChanged += InputFields_PropertyChanged;
        passwordValidator.PropertyChanged += InputFields_PropertyChanged;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        EmailEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
    }


    private async void LoginBtn_Clicked(object sender, EventArgs e)
    {
        string userToken = await _authServices.GetUserBearerTokenAsync(EmailEntry.Text, PasswordEntry.Text);

        if (userToken != String.Empty)
        {
            if(await _authServices.CanUserUseTheSystemAsync(userToken))
            {
                if (await SecureStorage.Default.GetAsync("loggedUserRole") == "Admin")
                {
                    AppShell.Current.Items.Where(shellItem => shellItem.Title == "Settings").FirstOrDefault().FlyoutItemIsVisible = true;
                }
                else
                {
                    AppShell.Current.Items.Where(shellItem => shellItem.Title == "Settings").FirstOrDefault().FlyoutItemIsVisible = false;
                }

                Shell.Current.GoToAsync("//StartPage");
            }
            else
            {
                DisplayAlert("Access denied", "You are yet to be given a role.", "Ok");
            }
        }
        else
        {
            DisplayAlert("Error", "Invalid credentials, check email and password ", "Ok");
        }
    }

    private void InputFields_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if ((emailValidator.IsValid && emailValidator.Value != null) &&
            (passwordValidator.IsValid && passwordValidator.Value != null))
        {
            LoginBtn.IsEnabled = true;
        }
        else
        {
            LoginBtn.IsEnabled = false;

        }
    }
}