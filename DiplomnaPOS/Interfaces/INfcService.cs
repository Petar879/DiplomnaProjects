using PCSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomnaPOS.Interfaces
{
    public interface INfcService
    {
        
        bool IsAnyReaderAvailable();
        SCardReader GetFirstOrDefaultCardReader(out string idleReaderName);
        bool GetCardAtr(SCardReader cardReader, out string result);

        List<KeyValuePair<int, string>> GetAllAvailableReaders();

        string GetCardAtrWithDefaultReader();
    }
}
