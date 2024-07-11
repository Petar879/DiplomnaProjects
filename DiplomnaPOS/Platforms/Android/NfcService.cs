using System;
using Android.App;
using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using DiplomnaPOS.Enums;
using DiplomnaPOS.Interfaces;
using PCSC;
using static Android.Nfc.NfcAdapter;
using Application = Microsoft.Maui.Controls.Application;


namespace DiplomnaPOS.Platforms
{
    public class NfcService : INfcService
    {
        public List<KeyValuePair<int, string>> GetAllAvailableReaders()
        {
            throw new NotImplementedException();
        }

        public string GetCardAtrWithDefaultReader()
        {
            throw new NotImplementedException();
        }

        public SCardReader GetFirstOrDefaultCardReader(out string idleReaderName)
        {
            throw new NotImplementedException();
        }

        //private readonly MainActivity mainActivity = (MainActivity)Platform.CurrentActivity;
        //private Lazy<NfcAdapter> lazynfcAdapter = new Lazy<NfcAdapter>(() => NfcAdapter.GetDefaultAdapter(Platform.CurrentActivity));
        //private NfcAdapter NfcAdapter => lazynfcAdapter.Value;
        //private PendingIntent pendingIntent;
        //private IntentFilter[] writeTagFilters;
        //private string[][] techList;
        ////private ReaderCallback readerCallback;
        //private NfcStatus NfcStatus => NfcAdapter == null ?
        //                      NfcStatus.Unavailable : NfcAdapter.IsEnabled ?
        //                      NfcStatus.Enabled : NfcStatus.Disabled;

        //public Task SendAsync(byte[] bytes)
        //{
        //    throw new NotImplementedException();
        //}
        bool INfcService.GetCardAtr(SCardReader cardReader, out string result)
        {
            throw new NotImplementedException();
        }

        bool INfcService.IsAnyReaderAvailable()
        {
            throw new NotImplementedException();
        }

    }
}
