using CommunityToolkit.Maui.Views;
using DiplomnaPOS.Services;
using System.ComponentModel;
using System.Text.Json.Nodes;

namespace DiplomnaPOS.Pages.Popups;

public partial class AddNewEmployeePopup : Popup
{
    private readonly AuthServices _authServices;


    public AddNewEmployeePopup(AuthServices authServices, List<string> rolesList)
	{
		InitializeComponent();
        OkBtn.IsEnabled = false;

        _authServices = authServices;

        emailValidator.PropertyChanged += InputFields_PropertyChanged;
        passwordConformationValidator.PropertyChanged += InputFields_PropertyChanged;
        passwordValidator.PropertyChanged += InputFields_PropertyChanged;

        rolesList.ForEach(role => rolePicker.Items.Add(role));
    }

    private async void OkBtn_Clicked(object sender, EventArgs e)
    {
        bool isBasicRegistrationSuccessful = await _authServices.RegisterUser(emailEntry.Text, passwordEntry.Text);

        if (rolePicker.SelectedItem != null)
        {
            bool isNewUserRoleSuccessful = await _authServices.AssignUserRole(emailEntry.Text, rolePicker.SelectedItem.ToString());
            if (isNewUserRoleSuccessful)
            {
                Close(true);
            }
            else
            {
                Close(false);
            }
        }
        else 
        {
            if (isBasicRegistrationSuccessful)
            {
                Close(true);
            }
            else
            {
                Close(false);
            }
        }
    }

    private void CancelBtn_Clicked(object sender, EventArgs e)
    {
        Close();
    }

    private void InputFields_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if ((emailValidator.IsValid && emailValidator.Value != null) &&
            (passwordConformationValidator.IsValid && passwordConformationValidator.Value != null) &&
            (passwordValidator.IsValid && passwordValidator.Value != null) &&
            (passwordConformationEntry.Text == passwordEntry.Text))
        {
            //IsFormValid = true;
            OkBtn.IsEnabled = true;
        }
        else
        {
            OkBtn.IsEnabled = false;
        }
    }
}