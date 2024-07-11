using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomnaPOS.Messages
{
    public class NfcStatusMessage : ValueChangedMessage<bool>
    {
        public NfcStatusMessage(bool value) : base(value)
        {
        }
    }
}
