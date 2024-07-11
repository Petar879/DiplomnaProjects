using CommunityToolkit.Maui.Views;
using DiplomnaPOS.Pages.Popups;
using EfLibrary.Models;
using System.Diagnostics;

namespace DiplomnaPOS.Pages;

public partial class UpdateMenuPage : ContentPage
{
	private UpdateMenuPageViewModel VM;

	public UpdateMenuPage(UpdateMenuPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		VM = viewModel;
        Loaded += OnPageLoaded;
    }


    protected async void OnPageLoaded(object sender, EventArgs e)
    {
        await VM.LoadDataAsync();
    }

}