

using CommunityToolkit.Mvvm.Messaging;
using DiplomnaPOS.Messages;
using Microsoft.Maui.Controls.Internals;
using System.Diagnostics;

namespace DiplomnaPOS.Pages;

public partial class DeviceManagerPage : ContentPage
{
    //NfcDeviceMonitor monitor = new();

    public DeviceManagerPage(DeviceManagerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        bool isThereAnyNfcDevice = (BindingContext as DeviceManagerViewModel).NfcDevices.Any();

        if (isThereAnyNfcDevice)
        {
            testNFCbtn.IsEnabled = isThereAnyNfcDevice;
        }
        else
        {
            testNFCbtn.IsEnabled = isThereAnyNfcDevice;

            DisplayAlert($"Error", "No readers found", "Ok");

        }

        WeakReferenceMessenger.Default.Register<NfcStatusMessage>(this, (r, m) =>
        {
            Dispatcher.DispatchAsync(new Action(() =>
            {
                testNFCbtn.IsEnabled = m.Value;

                if (m.Value)
                {
                    (BindingContext as DeviceManagerViewModel).GetNfcDeviceNames();
                    testNFCbtn.IsEnabled = true;
                }
                else
                {
                    testNFCbtn.IsEnabled = false;
                    (BindingContext as DeviceManagerViewModel).NfcDevices.Clear();
                }

            }));
        });
    }

    private async void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        RadioButton radioButton = sender as RadioButton;
        if (radioButton.IsChecked == true)
        {
            await (BindingContext as DeviceManagerViewModel).AssignNewNfcDevice(radioButton.Content.ToString());
        }
    }


}