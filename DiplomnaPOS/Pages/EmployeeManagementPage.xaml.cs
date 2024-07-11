using DiplomnaPOS.Pages;

namespace DiplomnaPOS.Pages;

public partial class EmployeeManagementPage : ContentPage
{
	private EmployeeManagementPageViewModel _viewModel;

	public EmployeeManagementPage(EmployeeManagementPageViewModel employeeManagementPageViewModel)
	{
		InitializeComponent();
		BindingContext = employeeManagementPageViewModel;
		_viewModel = employeeManagementPageViewModel;
	}
}