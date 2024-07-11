using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomnaPOS.Interfaces;
using DiplomnaPOS.Models;
using DiplomnaPOS.Pages.Popups;
using EfLibrary.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomnaPOS.Pages
{
    public partial class PartialRefundPageViewModel : ObservableObject
    {
        private DatabaseManager databaseManager = new();
        private Transaction _transaction;
        private INfcService _nfcService;

        [ObservableProperty]
        private string _pageTitle;

        [ObservableProperty]
        private ObservableCollection<TransactionItemModel> _transactionItems = new();

        private List<TransactionItemModel> itemIdsToBeRemovedFromTransactions = new();

        public bool IsPartialRefundPerformable;


        public PartialRefundPageViewModel(Transaction transaction, INfcService nfcService)
        {
            PageTitle = transaction.InvoiceId;
            _transaction = transaction;
            TransactionItems = databaseManager.GetTransactionItemsWithNames(transaction.Id).ToObservableCollection();
            IsPartialRefundPerformable = databaseManager.IsTransactionPartiallyRefundable(transaction.Id).Result;
            _nfcService = nfcService;
        }

        public async Task ReloadTransactionsList()
        {
            TransactionItems = databaseManager.GetTransactionItemsWithNames(_transaction.Id).ToObservableCollection();
        }

        [RelayCommand]
        public async Task PerformTransactionEdit()
        {
            string actoin = await Application.Current.MainPage.DisplayActionSheet("Refund to?", "Cancel", null, "Card", "Cash");

            foreach (var item in TransactionItems)
            {
                if (item.ProductQuantity == 0)
                {
                    itemIdsToBeRemovedFromTransactions.Add(item);
                }
            }

            TransactionItems = TransactionItems.Where(i => i.ProductQuantity != 0).ToObservableCollection();

            bool isThereAnDeletedProduct = TransactionItems.Any(tItem => tItem._productId == -1);

            switch (actoin)
            {
                case "Card":
                    var result = await Application.Current.MainPage.ShowPopupAsync(new AwaitingNfcPaymentPopup(_nfcService));

                    if (result != null)
                    {
                        await databaseManager.UpdateTransactionItemQuantity(_transaction, TransactionItems.ToList(), isThereAnDeletedProduct);
                        if (TransactionItems.Count() == itemIdsToBeRemovedFromTransactions.Count())
                        {
                            await databaseManager.DeleteTransactoion(_transaction.Id);
                        }
                        
                        await databaseManager.DeleteTranscationItems(_transaction.Id, itemIdsToBeRemovedFromTransactions);
                        await Application.Current.MainPage.DisplayAlert("Alert", "Card refund performed", "Ok");
                        await Shell.Current.GoToAsync("../");

                    }
                    break;

                case "Cash":

                    bool answer = await Application.Current.MainPage.DisplayAlert("Alert!", "Proceed with cash refund?", "Yes", "No");
                    if (answer)
                    {
                        await databaseManager.UpdateTransactionItemQuantity(_transaction, TransactionItems.ToList(), isThereAnDeletedProduct);
                        if (TransactionItems.Count() == 0)
                        {
                            await databaseManager.DeleteTransactoion(_transaction.Id);
                        }

                        await databaseManager.DeleteTranscationItems(_transaction.Id, itemIdsToBeRemovedFromTransactions);
                        await Application.Current.MainPage.DisplayAlert("Debug window", "Partial cash refund performed", "Ok");
                        await Shell.Current.GoToAsync("../");
                    }
                break;

                default:
                    Debug.WriteLine("No option was selected");
                break;
            }
        }

        [RelayCommand]
        public async Task RemoveItemFromTransaction(TransactionItemModel transactionItem)
        {
            itemIdsToBeRemovedFromTransactions.Add(transactionItem);
            TransactionItems.Remove(transactionItem);
        }
    }
}
