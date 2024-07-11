using CommunityToolkit.Maui.Views;

namespace DiplomnaPOS.Pages.Popups;

public partial class EditEmployeePopup : Popup
{
	public EditEmployeePopup()
	{
		InitializeComponent();
	}

    private void OkBtn_Clicked(object sender, EventArgs e)
    {

    }

    private void CancelBtn_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}