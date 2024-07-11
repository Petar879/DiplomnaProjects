using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DiplomnaPOS.Pages;

public partial class ReceiptManagerPage : ContentPage
{
	public ReceiptManagerPage()
	{
		InitializeComponent();

        XDocument doc = XDocument.Load(Path.Combine(FileSystem.Current.AppDataDirectory, "ReceiptConfigurationBlank.xml"));
        headerEditor.Text = doc.Root.Element("HeaderText").Value;
        //serverdByToggle.IsToggled = bool.Parse(doc.Root.Element("IsServerdByVisible").Value);
        footerEditor.Text = doc.Root.Element("FooterText").Value;   
    }

    private void SaveSettingsBtn_Pressed(object sender, EventArgs e)
    {

        try
        {
            XDocument doc = XDocument.Load(Path.Combine(FileSystem.Current.AppDataDirectory, "ReceiptConfigurationBlank.xml"));
            doc.Root.Element("HeaderText").Value = headerEditor.Text;
            //doc.Root.Element("IsServerdByVisible").Value = serverdByToggle.IsToggled.ToString();
            doc.Root.Element("FooterText").Value = footerEditor.Text;
            doc.Save(Path.Combine(FileSystem.Current.AppDataDirectory, "ReceiptConfigurationBlank.xml"));
            DisplayAlert("Success", "Receipt changes have been saved", "Ok");
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.ToString(), "Ok");
        }


    }


    //DisplayAlert("lol", Regex.Replace(editor.Text, @"\r\n?|\n", "/n"), "lol");


}