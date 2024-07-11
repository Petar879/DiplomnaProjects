using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomnaPOS.Models
{
    public partial class TransactionItemModel : ObservableObject
    {
        [ObservableProperty]
        public int _productId;

        [ObservableProperty]
        private string _productName;

        [ObservableProperty]
        private int _productInitialQuantity;

        [ObservableProperty]
        private int _productQuantity;

        public TransactionItemModel(int productId, string productName, int productQuantity)
        {
            ProductId = productId;
            ProductName = productName;
            ProductQuantity = productQuantity;
            ProductInitialQuantity = productQuantity;
        }
    }
}
