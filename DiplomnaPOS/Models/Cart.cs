using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using EfLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomnaPOS.Models
{
    public partial class Cart : ObservableObject
    {
        [ObservableProperty]
        private double productsInCartPrice = 0.0;

        [ObservableProperty]
        private ObservableCollection<CartProductModel> _productsCollection = new();

        public Cart()
        {
            ProductsInCartPrice = 0.0;
        }

        //public Cart(Cart cart)
        //{
        //    this.productsInCartPrice = cart.ProductsInCartPrice;
        //    this._productsCollection = cart.ProductsCollection;
        //}

        public void AddProduct(Product newProduct, int productQuantity)
        {
            var productName = newProduct.Name;

            if (ProductsCollection.Any(p => p.Product.Name == productName))
            {
                foreach (var product in ProductsCollection)
                {
                    if (product.Product.Name == productName && product.ProductCount < product.QuantityInInventory)
                    {
                        product.ProductCount++;
                        Debug.WriteLine($"{product.ProductCount} {product.Price}");
                        break;
                    }
                }
            }
            else
            {
                ProductsCollection.Add(new CartProductModel(newProduct, 1, (double)newProduct.Price, productQuantity));
            }

            ProductsInCartPrice = ProductsCollection.Sum(x => x.Price);
        }

        public void UpdateProductsInCartPrice()
        {
            ProductsInCartPrice = ProductsCollection.Sum(x => x.Price);
        }

        public void RemoveProduct(Product oldProduct)
        {   
            ProductsCollection = ProductsCollection.Where(p => p.Product.Name != oldProduct.Name).ToObservableCollection();
            UpdateProductsInCartPrice();
            //Debug.WriteLine("Items in cart");
            //foreach (var product in ProductsCollection)
            //{
            //    Debug.WriteLine(product.Product.Name);
            //}
        }
    }
}