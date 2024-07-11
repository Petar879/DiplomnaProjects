namespace DiplomnaPOS.Pages;

public partial class RefundsPage : ContentPage
{
	private RefundsPageViewModel _viewModel;
	public RefundsPage(RefundsPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		_viewModel = viewModel;
		//Loaded += OnPageLoaded;
		//Appearing += OnAppearing;
	}

    //private async void OnAppearing(object? sender, EventArgs e)
    //{
    //    base.OnAppearing();

    //}

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.LoadDataAsync();

    }
    //protected async void OnPageLoaded(object sender, EventArgs e)
    //{
    //    await _viewModel.LoadDataAsync();
    //}
}