using DiplomnaPOS.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomnaPOS.Converters
{
    public class DefaultNfcReaderTemplateConvertor : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate SpecialTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((KeyValuePair<int, string>)item).Value == Preferences.Get("nfc_default_device", string.Empty) ? SpecialTemplate : DefaultTemplate;
        }
    }
}

