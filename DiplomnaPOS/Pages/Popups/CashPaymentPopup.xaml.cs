using CommunityToolkit.Maui.Views;

namespace DiplomnaPOS.Pages.Popups;

public partial class CashPaymentPopup : Popup
{
    private double _cartPrice;
	public CashPaymentPopup(double priceFromCart)
	{
		InitializeComponent();
        _cartPrice = priceFromCart;
	}

    void OnNumberSelection(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        string btnPressed = button.Text;

        if (this.result.Text == "0")
        {
            this.result.Text = string.Empty;
        }

        this.result.Text += btnPressed;
        double number = double.Parse(this.result.Text);
        this.result.Text = number.ToString("N0");
    }

    void OnClear(object sender, EventArgs e)
    {
        this.result.Text = "0";
    }

    void OnBackspace(object sender, EventArgs e)
    {
        if (this.result.Text.Length > 1)
        {
            this.result.Text = this.result.Text.Remove(this.result.Text.Length - 1);
            double number = double.Parse(this.result.Text);
            this.result.Text = number.ToString("N0");
        }else
        {
            this.result.Text = "0";
        }
    }

    void OnClosePopup(object sender, EventArgs e)
    {
        this.Close();
    }

    void OnCalculateChange(object sender, EventArgs e)
    {
        double calculatorValue = double.Parse(this.result.Text);
        if (calculatorValue >= _cartPrice)
        {
           this.CloseAsync((calculatorValue - _cartPrice));
        }
        else
        {
           Application.Current.MainPage.DisplayAlert("Error", "Less than price in cart", "Ok");
        }
        
    }
}