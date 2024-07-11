using DiplomnaPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomnaPOS.Converters
{
    internal class UnassignedUserTemplateConverter : DataTemplateSelector
    {
        public DataTemplate MissingRoleTemplate { get; set; }

        public DataTemplate DefaultRoleTemplate { get; set; }
        
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((UsersDataModel)item).Role == null ? MissingRoleTemplate : DefaultRoleTemplate;
        }
    }
}
