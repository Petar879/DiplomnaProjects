using DiplomnaPOS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCSC.Utils;
using PCSC;
using PCSC.Iso7816;
using System.Formats.Tar;
using System.Diagnostics;

namespace DiplomnaPOS.Platforms
{ 
    public class InfcService : INfcService
    {
        public List<KeyValuePair<int, string>> GetAllAvailableReaders()
        {
            List<KeyValuePair<int, string>> readerNamesKVP = new();
            using (var context = new SCardContext())
            {
                context.Establish(SCardScope.System);

                // Get the list of smart card readers
                var readerNames = context.GetReaders();
                if (readerNames == null || readerNames.Length == 0)
                {
                    Debug.WriteLine("No smart card readers found.");
                    return null;
                }

                ////Console.WriteLine("Available smart card readers: ");
                //foreach (var readerName in readerNames)
                //{
                //    //Console.WriteLine($"- {readerName}");
                //    readerNamesList.Add(readerName);
                //}

                for (int i = 0; i < readerNames.Count(); i++)
                {
                    readerNamesKVP.Add(new KeyValuePair<int, string>(i, readerNames[0]));
                }

                return readerNamesKVP;
            }
        }

        // DEAD CODE
        public bool GetCardAtr(SCardReader cardReader, out string result)
        {
            try
            {
                byte[] atr = Array.Empty<byte>();

                var card = cardReader.GetAttrib(SCardAttribute.AtrString, out atr);
                result =  BitConverter.ToString(atr);
                return true;
            }
            catch (Exception exception)
            {
                result = exception.Message;
                return false;
            }
        }

        public string GetCardAtrWithDefaultReader()
        {
            using (var context = new SCardContext())
            {
                context.Establish(SCardScope.System);
                var readerNames = context.GetReaders();
                // Connect to the first available reader and check the status
                using (var reader = new SCardReader(context))
                {
                    //var readerName = readerNames[0];

                    var readerName = Preferences.Get("nfc_default_device", string.Empty);

                    if (readerName == null || !readerNames.Contains(readerName))
                    {
                        return null;
                    }

                    var sc = reader.Connect(readerName, SCardShareMode.Shared, SCardProtocol.Any);
                    
                    if (sc == SCardError.Success)
                    {
                        byte[] atr = Array.Empty<byte>();

                        var card = reader.GetAttrib(SCardAttribute.AtrString, out atr);
                        return BitConverter.ToString(atr);

                        //try
                        //{
                        //    byte[] atr = Array.Empty<byte>();

                        //    var card = reader.GetAttrib(SCardAttribute.AtrString, out atr);
                        //    return BitConverter.ToString(atr);
                        //}
                        //catch (Exception exception)
                        //{
                        //    result = exception.Message;
                        //    return false;
                        //}

                        //DisplayAlert("Error", $"Could not connect to reader {readerName}: {SCardHelper.StringifyError(sc)}", "ok");
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public SCardReader GetFirstOrDefaultCardReader(out string idleReaderName)
        {
            using (var context = new SCardContext())
            {
                context.Establish(SCardScope.System);
                var readerNames = context.GetReaders();
                // Connect to the first available reader and check the status
                using (var reader = new SCardReader(context))
                {
                    var readerName = readerNames[0];
                    var sc = reader.Connect(readerName, SCardShareMode.Shared, SCardProtocol.Any);
                    idleReaderName = readerName;
                    if (sc != SCardError.Success)
                    {
                        idleReaderName = readerName;
                        //DisplayAlert("Error", $"Could not connect to reader {readerName}: {SCardHelper.StringifyError(sc)}", "ok");
                        return null;
                    }
                    else
                    {
                        return reader;
                    }
                }
            }
        }

        public bool IsAnyReaderAvailable()
        {
            using (var context = new SCardContext())
            {
                context.Establish(SCardScope.System);

                // Get the list of smart card readers
                var readerNames = context.GetReaders();
                if (readerNames == null || readerNames.Length == 0)
                {
                    //DisplayAlert("Message", "No smart card readers found.", "ok");
                    return false;
                }
            }
            return true;
        }
    }
}
