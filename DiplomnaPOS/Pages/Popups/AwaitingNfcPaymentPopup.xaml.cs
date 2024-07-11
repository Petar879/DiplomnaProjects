using CommunityToolkit.Maui.Views;
using DiplomnaPOS.Enums;
using DiplomnaPOS.Interfaces;
using PCSC.Monitoring;
using System.Diagnostics;

#if WINDOWS
using DiplomnaPOS.Platforms.Windows;

#endif

namespace DiplomnaPOS.Pages.Popups;

public partial class AwaitingNfcPaymentPopup : Popup
{
    INfcService _nfcService;
	public AwaitingNfcPaymentPopup(INfcService nfcService)
	{
		InitializeComponent();
        _nfcService = nfcService;


#if WINDOWS

        SmartCardMonitor monitor = new();
        string cardAtr = string.Empty;
        monitor.DeviceConnected += (sender, e) =>
        {
            Debug.WriteLine("Card connected.");
            cardAtr = _nfcService.GetCardAtrWithDefaultReader();

            monitor.StopMonitoring();

            // Somehow this calls the UI thread
            Dispatcher.DispatchAsync(new Action(() =>
            {
                Close(cardAtr);

                //border.Stroke = (Color)Application.Current.Resources["ContainerBorderColor"];// Change back to red
            }));
        };

#elif ANDROID
            Device.BeginInvokeOnMainThread(() =>
            {
                Close();
            });    
#endif

        //monitor.DeviceDisconnected += (sender, e) =>
        //{
        //    Debug.WriteLine("Card disconnected.");
        //};

        //if (cardAtr != string.Empty)
        //{

        //    monitor.StopMonitoring();

        //}


    }


    protected override async Task OnDismissedByTappingOutsideOfPopup(CancellationToken token = default)
    {
        await CloseAsync(null, token);
    }

    private void closePopUpBtn_Clicked(object sender, EventArgs e)
    {
        CloseAsync(null);

    }


}