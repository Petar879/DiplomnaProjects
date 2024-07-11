

namespace DiplomnaPOS.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}


    //private void ReceiptManagerBtn_Clicked(object sender, EventArgs e)
    //{
    //    Shell.Current.GoToAsync("ReceiptManagerPage");
    //}


    private void UpdateMenuBtn_Tapped(object sender, TappedEventArgs e)
    {
        Shell.Current.GoToAsync("UpdateMenuPage");
    }

    private void RefundsBtn_Tapped(object sender, TappedEventArgs e)
    {
        Shell.Current.GoToAsync("RefundsPage");
    }

    private void ReceiptManagerBtn_Tapped(object sender, TappedEventArgs e)
    {
        Shell.Current.GoToAsync("ReceiptManagerPage");
    }

    private void EmployeeManaganerBtn_Tapped(object sender, TappedEventArgs e)
    {
        Shell.Current.GoToAsync("EmployeeManagementPage");
    }
}