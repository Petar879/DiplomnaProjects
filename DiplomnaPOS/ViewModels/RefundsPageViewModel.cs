using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomnaPOS.Interfaces;
using DiplomnaPOS.Models;
using DiplomnaPOS.Pages.Popups;
using EfLibrary.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomnaPOS.Pages
{
    public partial class RefundsPageViewModel : ObservableObject
    {
        private DatabaseManager _databaseManager = new();
        private INfcService _nfcService;

        [ObservableProperty]
        private List<Transaction> _transactionsList = new();

        //[ObservableProperty]
        //private List<TransactionItem> _transactionItemsList = new();

        // For unit testing
        public RefundsPageViewModel() { }

        public RefundsPageViewModel(INfcService nfcService)
        {
            _nfcService = nfcService;
        }

        public async Task LoadDataAsync()
        {
            TransactionsList = await _databaseManager.GetTransactions();
            TransactionsList = TransactionsList.OrderByDescending(t => t.Date).ToList();
            //TransactionItemsList = await _databaseManager.GetTransactionItems();
        }

        [RelayCommand]
        public async Task InitiateFullRefund(Transaction transaction)
        {

            if(await _databaseManager.IsTransactiontFullyRefundable(transaction.Id))
            {
                bool answer = await Application.Current.MainPage.DisplayAlert("Confirm full refund ?", $"You are about to fully refund the order to the customer.\r\nProceed? ", "Yes", "No");
                if (answer)
                {
                    string actoin = await Application.Current.MainPage.DisplayActionSheet("Refund to?", "Cancel", null, "Card", "Cash");

                    switch (actoin)
                    {
                        case "Card":
                            var result = await Application.Current.MainPage.ShowPopupAsync(new AwaitingNfcPaymentPopup(_nfcService));

                            if (result != null)
                            {
 
                                await Application.Current.MainPage.DisplayAlert("Alert", "Card refund performed", "Ok");
                                await _databaseManager.DeleteTransactoion(transaction.Id);
                                LoadDataAsync();

                            }
                        break;

                        case "Cash":

                            bool answerCash = await Application.Current.MainPage.DisplayAlert("Alert!", "Proceed with cash refund?", "Yes", "No");
                            if (answerCash)
                            {
                                Application.Current.MainPage.DisplayAlert("Debug window", "Partial cash refund performed", "Ok");
                                await _databaseManager.DeleteTransactoion(transaction.Id);
                                LoadDataAsync();
                            }
                        break;

                        default:
                            Debug.WriteLine("No option was selected");
                        break;
                    }


                    //await Application.Current.MainPage.DisplayAlert("Alert!", $"Here should be the full refund for {transaction.InvoiceId}", "Ok");

                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "An unknown product is present in this transaction.\nFull refund is not possible.", "Ok");

            }
        }

        [RelayCommand]
        public async Task InitiatePartialRefund(Transaction transaction)
        {
            await Shell.Current.Navigation.PushAsync(new PartialRefundPage(transaction, _nfcService));
        }

    }
}
