namespace DiplomnaPOS.Pages;
using CommunityToolkit.Maui;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel viewModel)
	{
        InitializeComponent();
		BindingContext = viewModel;
        Loaded += OnPageLoaded;
	}


    protected async void OnPageLoaded(object sender, EventArgs e)
    {
        await (BindingContext as DashboardViewModel).LoadDataAsync();
    }
}