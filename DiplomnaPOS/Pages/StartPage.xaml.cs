using CommunityToolkit.Mvvm.Messaging;
using DiplomnaPOS.Helpers;
using DiplomnaPOS.Interfaces;
using DiplomnaPOS.Messages;
using DiplomnaPOS.Models;
using DiplomnaPOS.Pages.AuthenticationPages;
using DiplomnaPOS.Services;
using EfLibrary.Models;
using Microsoft.Maui.Controls;
using PCSC;
using System.Diagnostics;

namespace DiplomnaPOS.Pages;

public partial class StartPage : ContentPage
{
    private readonly INfcService _myService;
    private readonly AuthServices _authService;


    private StartPageViewModel VM;
    public StartPage(StartPageViewModel viewModel, INfcService nfcService, AuthServices authService)
	{
		InitializeComponent();
        VM = viewModel;
		BindingContext = viewModel;

        _myService = nfcService;
        NfcBtn.IsEnabled = nfcService.IsAnyReaderAvailable();

        _authService = authService;

        WeakReferenceMessenger.Default.Register<NfcStatusMessage>(this, (r, m) =>
        {
            Dispatcher.DispatchAsync(new Action(() =>
            {
                NfcBtn.IsEnabled = m.Value;
            }));
        });

    }

    public StartPage(StartPageViewModel viewModel, AuthServices authService)
    {
        InitializeComponent();
        VM = viewModel;
        BindingContext = viewModel;

        //_myService = nfcService;
        NfcBtn.IsEnabled = false;

        _authService = authService;

        WeakReferenceMessenger.Default.Register<NfcStatusMessage>(this, (r, m) =>
        {
            Dispatcher.DispatchAsync(new Action(() =>
            {
                NfcBtn.IsEnabled = m.Value;
            }));
        });

    }


    //protected async override void OnAppearing()
    //{
    //    base.OnAppearing();
    //    await VM.LoadDataAsync();



    //    //var selectedValue = RadioButtonGroup.GetSelectedValue(categoryNamesScrollView);
    //    //if (selectedValue != null)
    //    //{
    //    //    Debug.WriteLine(selectedValue.ToString());
    //    //}

    //}

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await VM.LoadDataAsync();

    }


    //protected async void OnPageLoaded(object sender, EventArgs e)
    //{
    //    await (BindingContext as StartPageViewModel).LoadDataAsync();
    //}

    //private void NfcBtn_Clicked(object sender, EventArgs e)
    //{
    //    if (_myService.IsAnyReaderAvailable())
    //    {
    //        string readerName = String.Empty;
    //        SCardReader reader = _myService.GetFirstOrDefaultCardReader(out readerName);


    //        string response = String.Empty;


    //        if (reader != null)
    //        {
    //            DisplayAlert("Yipee", $"Atr is\n{_myService.GetCardAtrWithDefaultReader()}", "Nah, I'd win");
    //        }
    //        else
    //        {
    //            DisplayAlert("Error", $"Reader {readerName} is idle", "Nah, I'd win");
    //        }
    //    }
    //    else
    //    {
    //        DisplayAlert("Error", "No reader available", "Nah, I'd win");
    //    }
    //}

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var radioButton = (RadioButton)sender;
        VM.CategoryId = (int)radioButton.Value;
        
    }

    private void ProductQuantityStepper_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        var stepper = (Stepper)sender;
        var item = (CartProductModel)stepper.BindingContext;

        //DisplayAlert("Alert", item.Product.Name, "Ok");

        var currentProduct = VM.ProductsAfterFiltering.FirstOrDefault(p => p.Product.Id == item.Product.Id);

        if (currentProduct != null)
        {
            currentProduct.ProductCount = (int)e.NewValue;
        }

        //VM.ProductsAfterFiltering.FirstOrDefault(p => p.Product.Id == item.Product.Id).ProductCount = (int)e.NewValue;

        VM.ProductCart.UpdateProductsInCartPrice();
    }

    private void ItemBorder_Tapped(object sender, TappedEventArgs e)
    {
        var border = (Border)sender; // Cast the sender to a Border

        // Initial color
        border.Stroke = (Color)Application.Current.Resources["Primary"];

        // Create a Timer with a 200 millisecond interval
        var timer = new System.Timers.Timer(150);
        timer.AutoReset = false; // So it only fires once
        timer.Elapsed += (s, timerE) =>
        {
            Dispatcher.DispatchAsync(new Action(() =>
            {
                border.Stroke = (Color)Application.Current.Resources["ContainerBorderColor"];// Change back to red
            }));
        };

        timer.Start(); // Start the timer to change it back
    }

    private void TestingPageShortcut_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("Lol", "Nothing to test yet", "Lol");
        //Shell.Current.GoToAsync("/ReceiptManagerPage");
    }

    private void LogOutBtn_Clicked(object sender, EventArgs e)
    {
        _authService.Logout();
        Shell.Current.GoToAsync($"//LoginPage");
    }
}