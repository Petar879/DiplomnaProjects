using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomnaPOS.Interfaces;
using DiplomnaPOS.Models;
using DiplomnaPOS.Pages.Popups;
using EfLibrary.Models;
using Microsoft.Maui.Animations;
using PCSC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DiplomnaPOS.Pages
{
    public partial class StartPageViewModel : ObservableObject
    {
        private readonly INfcService _nfcService;
        private DatabaseManager databaseManager = new();
        private Cart previousCart = new();
        private PrinterManager printerManager = new();

        public List<ProductsQuantity> productsQuantityFromDb = new();
        public List<Product> productsFromDb = new();

        #region Observable properties
        [ObservableProperty]
        public int categoryId;

        [ObservableProperty]
        public List<Category> categoryList = new();

        [ObservableProperty]
        public List<CartProductModel> productsAfterFiltering = new();

        [ObservableProperty]
        public Cart productCart = new();
        #endregion


        // For unit testing
        public StartPageViewModel() { }

        public StartPageViewModel(Cart _cart, INfcService nfcService) 
        {
            _nfcService = nfcService;
            ProductCart = _cart;
            //databaseManager.TestDBFunctionality();
        }

        public async Task LoadDataAsync()
        {
            CategoryList = await databaseManager.GetCategories();
            productsQuantityFromDb = await databaseManager.GetProductsQuantities();
            productsFromDb = await databaseManager.GetProducts();

            await UpdateFilteredCollection();
        }

        partial void OnCategoryIdChanged(int value)
        {
            Task.Run(() => UpdateFilteredCollection()).Wait();
        }

        //[RelayCommand]
        public async Task UpdateFilteredCollection()
        {
            if (!productsFromDb.Any())
            {
                productsFromDb = await databaseManager.GetProducts();
                productsQuantityFromDb = await databaseManager.GetProductsQuantities();
            }

            List<CartProductModel> tmpList = new();

            foreach (var item in productsFromDb.Where(p => p.CategoryId == CategoryId).ToList())
            {
                var tmpQuantity = productsQuantityFromDb.FirstOrDefault(pq => pq.ProductId == item.Id);

                tmpList.Add(new CartProductModel(item, 0, 0, tmpQuantity.ProductQuantity.Value));
            }

            foreach (var categoryProduct in tmpList)
            {
                foreach (var cartProduct in ProductCart.ProductsCollection)
                {
                    if (categoryProduct.Product.Id == cartProduct.Product.Id)
                    {
                        var tmpProductCount = cartProduct.ProductCount;
                        categoryProduct.ProductCount = tmpProductCount--;
                    }
                }
            }

            ProductsAfterFiltering = tmpList;
            //ProductsAfterFiltering = productsFromDb.Where(p => p.CategoryId == CategoryId).ToList();
        }

        [RelayCommand]
        public async Task AddProductToCart(CartProductModel newProduct)
        {
            //int currentProductQuantityInInventory = (int)productsQuantityFromDb.FirstOrDefault(p => p.ProductId == newProduct.Product.Id).ProductQuantity;
            //if (currentProductQuantityInInventory > 0)
            int currentProductQuantityInInventory = newProduct.QuantityInInventory + newProduct.ProductCount;

            if (newProduct.ProductCount < currentProductQuantityInInventory)
            {
                if (newProduct.ProductCount == 0)
                {
                    newProduct.ProductCount++;
                }

                //productsQuantityFromDb.FirstOrDefault(pqf => pqf.ProductId == newProduct.Product.Id).ProductQuantity--;

                Debug.WriteLine($"Added {newProduct.Product.Name}");
                Product tmpProduct = newProduct.Product;
                ProductCart.AddProduct(tmpProduct, newProduct.QuantityInInventory);
                //newProduct.ProductCount++;
            }
        }

        [RelayCommand]
        public async Task CancelOrder()
        {
            if(ProductCart.ProductsCollection.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Alert!", "No order to cancel.", "Ok");
            }
            else
            {
                bool answer = await Application.Current.MainPage.DisplayAlert("Alert!", "Do you want to cancel the order?", "Yes", "No");
                if (answer)
                {
                    ProductsAfterFiltering.ForEach(item => item.ResetProductCount());

                    ProductCart.ProductsCollection.Clear();
                    ProductCart.ProductsInCartPrice = 0;
                }
            }
        }

        [RelayCommand]
        public void RemoveProductFromCart(CartProductModel product)
        {
            //productsQuantityFromDb.FirstOrDefault(pqf => pqf.ProductId == product.Product.Id).ProductQuantity += product.ProductCount;
            try
            {
                ProductsAfterFiltering.First(cpm => cpm.Product.Id == product.Product.Id).ProductCount = 0;
            }
            catch (Exception)
            {

                
            }

            ProductCart.RemoveProduct(product.Product);
        }

        [RelayCommand]
        public async Task PerformCardPayment()
        {
            if (ProductCart.ProductsCollection.Count > 0)
            {
                var result = await Application.Current.MainPage.ShowPopupAsync(new AwaitingNfcPaymentPopup(_nfcService));

                if (result != null)
                {
                    if (result is string stringResult)
                    {
                        await databaseManager.AddNewTranscation(1, ProductCart, stringResult);
                        await Application.Current.MainPage.DisplayAlert($"Success", $"Thank you for your purchase.\nHave a nice day!", "Ok");

                        PrinterManager printerManager = new();
                        printerManager.GenerateReceipt(ProductCart);

                        //previousCart = new Cart(ProductCart);

                        await CleanupAfterPerformedTransaction();
                    }
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert($"Error", $"No items are selected", "Ok");
            }
        }

        [RelayCommand]
        public async Task PerformCashPayment()
        {
            if(ProductCart.ProductsCollection.Count > 0)
            {
                var result = await Application.Current.MainPage.ShowPopupAsync(new CashPaymentPopup(ProductCart.ProductsInCartPrice));
                if (result != null)
                {
                    if (result is double doubleResult)
                    {
                        await databaseManager.AddNewTranscation(1, ProductCart);
                        await Application.Current.MainPage.DisplayAlert($"Success", $"Change\n{Math.Round(doubleResult, 2) }", "Ok");

#if WINDOWS

                        printerManager.GenerateReceipt(ProductCart, doubleResult);

#endif

                        await CleanupAfterPerformedTransaction();
                    }
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert($"Error", $"No items are selected", "Ok");
            }
        }

        [RelayCommand]
        public async Task PerformRecieptReprint()
        {
            if(previousCart.ProductsCollection.Count > 0)
            {
                printerManager.GenerateReprint(previousCart);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert($"Error", $"No transaction was made", "Ok");
            }
        }

        private async Task CleanupAfterPerformedTransaction()
        {
            previousCart.ProductsInCartPrice = ProductCart.ProductsInCartPrice;
            foreach (var item in ProductCart.ProductsCollection)
            {
                previousCart.ProductsCollection.Add(new CartProductModel(item.Product, item.ProductCount, item.Price, 0));
            }

            ProductsAfterFiltering.ForEach(item => item.SubtractProductCountFromQuantity());


            ProductCart.ProductsCollection.Clear();
            ProductCart.ProductsInCartPrice = 0;
        }
    }
}
