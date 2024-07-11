using CommunityToolkit.Mvvm.ComponentModel;
using EfLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomnaPOS.Models
{
    public partial class CartProductModel : ObservableObject
    {
        [ObservableProperty]
		private Product _product;

        [ObservableProperty]
		private int _productCount;

        [ObservableProperty]
        private double _price;

        [ObservableProperty]
        private int _quantityInInventory;

        public CartProductModel()
        {
            Product = new Product();
            ProductCount = 0;
            Price = 0;
            QuantityInInventory = 0;
        }

        public CartProductModel(Product  product, int productCount, double price, int quantityInInventory)
        {
            Product = product;
            ProductCount = productCount;
            Price = price;
            QuantityInInventory = quantityInInventory;
        }

        public void SubtractProductCountFromQuantity()
        {
            QuantityInInventory -= ProductCount;
            ProductCount = 0;
        }

        public void ResetProductCount()
        {
            ProductCount = 0; 
        }
    
        partial void OnProductCountChanged(int value)
        {
            Price = (double)Product.Price.Value * value;
        }

    }
}
