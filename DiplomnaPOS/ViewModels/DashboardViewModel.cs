
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomnaPOS.Models;
using EfLibrary.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DiplomnaPOS.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        private DatabaseManager databaseManager = new();

        #region Observable properties

        [ObservableProperty]
        public bool isDataAvailable = false;

        [ObservableProperty]
        public bool isErrorMessageShown = false;

        [ObservableProperty]
        public bool isLoading = true;

        [ObservableProperty]
        public List<Transaction> transactionsList = new();

        //[ObservableProperty]
        //public List<string?> mostSoldProductsList = new();

        //[ObservableProperty]
        //public Dictionary<string, int?> productsQuantities = new();

        #endregion

        
        //public Command TestCommand { get; set; }

        public DashboardViewModel()
        {

            //TestCommand = new Command(
            //    execute: () =>
            //    {
            //        MainThread.InvokeOnMainThreadAsync(async () =>
            //        {
            //            IsLoading = true;
            //            TransactionsList = await databaseManager.GetTransactions();
            //            MostSoldProductsList = await databaseManager.GetMostPurchasedProducts();
            //            IsLoading = false;

            //            IsDataAvailable = true;
            //            IsErrorMessageShown = false;
            //        });
            //    },
            //    canExecute: () =>
            //    {
            //        IsDataAvailable = false;
            //        IsErrorMessageShown = true;
            //        return databaseManager.IsDatabaseConnected();
            //    });
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            //https://blog.ewers-peters.de/mvvm-source-generators-advanced-scenarios
            try
            {
                //MostSoldProductsList = await databaseManager.GetMostPurchasedProducts();
                //ProductsQuantities.Clear();
                IsLoading = false;
                TransactionsList = await databaseManager.GetTransactions();
                TransactionsList = TransactionsList.OrderByDescending(t => t.Date).ToList();
                //ProductsQuantities = await databaseManager.GetProductsQuantitiesWithProductNamesAsync();

                IsErrorMessageShown = false;
                IsDataAvailable = true;
            }
            catch (Exception)
            {
                IsLoading = false;
                IsErrorMessageShown = true;
                IsDataAvailable = false;
            }

            
            //if (await databaseManager.IsDatabaseConnected())
            //{

            //}
            //else
            //{

            //}
        }



    }
}
