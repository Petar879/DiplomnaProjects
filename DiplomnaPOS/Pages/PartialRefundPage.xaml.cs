using DiplomnaPOS.Interfaces;
using EfLibrary.Models;

namespace DiplomnaPOS.Pages;

public partial class PartialRefundPage : ContentPage
{
	private PartialRefundPageViewModel _viewModel;
    public PartialRefundPage(Transaction transaction, INfcService nfcService)
	{
		InitializeComponent();
		_viewModel = new PartialRefundPageViewModel(transaction, nfcService);
		BindingContext = _viewModel;
        performRefundBtn.IsEnabled = _viewModel.IsPartialRefundPerformable;
        Loaded += OnPageLoaded;
    }

    protected async void OnPageLoaded(object sender, EventArgs e)
    {
       await (BindingContext as PartialRefundPageViewModel).ReloadTransactionsList();
    }

    private void cancelOperationBtn_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    
}