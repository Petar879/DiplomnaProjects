using DiplomnaPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomnaPOS.Converters
{
    class TransactionItemTemplateConverter : DataTemplateSelector
    {
        public DataTemplate DefaultItemTemplate {  get; set; }
        public DataTemplate MissingItemTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            
            return ((TransactionItemModel)item).ProductId == -1 ? MissingItemTemplate : DefaultItemTemplate;
        }
    }
}
