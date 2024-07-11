using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DiplomnaPOS.Interfaces;
using DiplomnaPOS.Messages;
using DiplomnaPOS.Models;
using DiplomnaPOS.Pages.Popups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomnaPOS.Pages
{
    public partial class DeviceManagerViewModel : ObservableObject
    {
        private readonly INfcService _nfcService;
        

        [ObservableProperty]
        public ObservableCollection<KeyValuePair<int, string>> _nfcDevices = new();

        [ObservableProperty]
        private string currentNfcDeviceName = null;


        public DeviceManagerViewModel(INfcService nfcService)
        {
            _nfcService = nfcService;

            CurrentNfcDeviceName = Preferences.Get("nfc_default_device", string.Empty);

            //Task task = GetNfcDeviceNames();

            Task.Run(async () => await GetNfcDeviceNames());


            //WeakReferenceMessenger.Default.Register<NfcStatusMessage>(this, (r, m) =>
            //{
                

            //    Dispatcher.Dispatch(new Action(() =>
            //    {
            //        IsNfcDevicePresent = m.Value;

            //        if (m.Value)
            //        {
            //            GetNfcDeviceNames();
            //        }
            //        else
            //        {
            //            Application.Current.MainPage.DisplayAlert($"Error", "No readers found", "Ok");
            //            NfcDevices.Clear();
            //        }

            //        //border.Stroke = (Color)Application.Current.Resources["ContainerBorderColor"];// Change back to red

            //    }));


            //    //Device.BeginInvokeOnMainThread(async () =>
            //    //{

            //    //});
            //});

        }

        [RelayCommand]
        public async Task GetNfcDeviceNames()
        {
            //await Task.Delay(10000);

            var _devices = _nfcService.GetAllAvailableReaders();

            if (_devices != null)
            {
                NfcDevices = _devices.ToObservableCollection();

                //Testing data
                //NfcDevices.Add(new KeyValuePair<int, string>(1, "Lol1"));
                //NfcDevices.Add(new KeyValuePair<int, string>(2, "Lol2"));
                //NfcDevices.Add(new KeyValuePair<int, string>(3, "Lol3"));
                //NfcDevices.Add(new KeyValuePair<int, string>(4, "Lol4"));
            }
        }

        [RelayCommand]
        public async Task AssignNewNfcDevice(string nfcDeviceName)
        {
            Preferences.Set("nfc_default_device", nfcDeviceName);
            CurrentNfcDeviceName = nfcDeviceName;
            await Application.Current.MainPage.DisplayAlert($"New default device selected", $" {Preferences.Get("nfc_default_device", string.Empty)}", "Lol");
        }


        [RelayCommand]
        public async Task TestNfcDevice()
        {
            string _cardUid = _nfcService.GetCardAtrWithDefaultReader();

            if (NfcDevices.Any())
            {
                var result = await Application.Current.MainPage.ShowPopupAsync(new AwaitingNfcPaymentPopup(_nfcService));

                if (result != null)
                {
                    if (result is string stringResult)
                    {
                        await Application.Current.MainPage.DisplayAlert($"Success", $"Thank you for your purchase.\nHave a nice day!", "Ok");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert($"Error", $"There was an issue with the card reader", "Ok");
                    }
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert($"Error", $"No readers are connected", "Ok");
            }
        }
    

    }
}
