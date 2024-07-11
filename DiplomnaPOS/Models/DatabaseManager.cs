using CommunityToolkit.Maui.Core.Extensions;
using EfLibrary.Data;
using EfLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomnaPOS.Models
{
    public class DatabaseManager
    {
        //TODO
        // Connect to different database ip's 
        // Add other getters
        // 

        DiplomnaContext context;

        public DatabaseManager()
        {
            context = new();
        }

        public DatabaseManager(DiplomnaContext _context)
        {
            context = _context;
        }

        #region Database Create
        public async Task AddNewCategory(string newCategoryName)
        {
            var newCategory = new Category { Name = newCategoryName };
            context.Categories.Add(newCategory);
            context.SaveChanges();
        }

        public async Task AddNewProduct(Product newProduct)
        {
            context.Products.Add(newProduct);
            context.SaveChanges();
        }

        public async Task AddNewTranscation(int employeeId, Cart currentCart)
        {
            var currentDateTime = DateTime.Now;
            var newTransaction = new Transaction
            {
                EmployeeId = employeeId,
                Date = currentDateTime,
                InvoiceId = $"{currentDateTime.Millisecond}{currentDateTime.Day}{currentDateTime.Month}{currentDateTime.Year}",
                PaymentMethod = "cash",
                PayerCard = "-",
                PaymentValue = (decimal?)currentCart.ProductsInCartPrice
            };

            foreach (var item in currentCart.ProductsCollection)
            {
                await UpdateProductQuantity(item.Product.Id, item.ProductCount, '-');
            }

            context.Transactions.Add(newTransaction);
            context.SaveChanges();

            foreach (var item in currentCart.ProductsCollection)
            {
                await AddNewTransactionItem(newTransaction.Id, item.Product.Id, item.ProductCount);
            }
        }

        public async Task AddNewTranscation(int employeeId, Cart currentCart, string nfcAtr)
        {
            var currentDateTime = DateTime.Now;
            var newTransaction = new Transaction
            {
                EmployeeId = employeeId,
                Date = currentDateTime,
                InvoiceId = $"{currentDateTime.Millisecond}{currentDateTime.Day}{currentDateTime.Month}{currentDateTime.Year}",
                PaymentMethod = "card",
                PayerCard = nfcAtr,
                PaymentValue = (decimal?)currentCart.ProductsInCartPrice
            };

            foreach (var item in currentCart.ProductsCollection)
            {
                await UpdateProductQuantity(item.Product.Id, item.ProductCount, '-');
            }

            context.Transactions.Add(newTransaction);
            context.SaveChanges();

            foreach (var item in currentCart.ProductsCollection)
            {
                await AddNewTransactionItem(newTransaction.Id, item.Product.Id, item.ProductCount);
            }
        }

        public async Task AddNewTransactionItem(int transationId, int productId, int quantity)
        {
            var newTransactionItem = new TransactionItem
            {
                TransactionId = transationId,
                ProductId = productId,
                Quantity = quantity
            };

            context.TransactionItems.Add(newTransactionItem);
            context.SaveChanges();
        }

        #endregion

        #region Database Read
        public async Task<bool> IsDatabaseConnected()
        {
            return context.Database.CanConnect() ? true : false;
        }

        public async Task<List<Product>> GetProducts()
        {
            await context.DisposeAsync();
            context = new();
            return await context.Products.ToListAsync();
        }

        public async Task<List<Category>> GetCategories()
        {
            await context.DisposeAsync();
            context = new();
            return await context.Categories.ToListAsync();
        }

        public async Task<List<Transaction>> GetTransactions()
        {
            await context.DisposeAsync();
            context = new();
            return await context.Transactions.ToListAsync();
        }

        public async Task<List<ProductsQuantity>> GetProductsQuantities()
        {
            await context.DisposeAsync();
            context = new();
            return await context.ProductsQuantities.ToListAsync();
        }

        public async Task<List<TransactionItem>> GetTransactionItems()
        {
            await context.DisposeAsync();
            context = new();
            return await context.TransactionItems.ToListAsync();
        }

        public async Task<int> GetSpecificProductQuantity(int proudctId)
        {
            await context.DisposeAsync();
            context = new();

            var productQuantity = await context.ProductsQuantities.FirstOrDefaultAsync(p => p.ProductId == proudctId);

            return (int)productQuantity.ProductQuantity;
        }

        private async Task<List<TransactionItem>> GetTransactionItemsForSpecificTransaction(int transactionId)
        {
            await context.DisposeAsync();
            context = new();

            return await context.TransactionItems.Where(t => t.TransactionId == transactionId).ToListAsync();
        }

       

        //public async Task<List<string?>> GetMostPurchasedProducts()
        //{
        //    var result = await context.TransactionItems.Include(x => x.Product)
        //                                         .GroupBy(x => x.Product.Name)
        //                                         .Select(g => new { Name = g.Key, Quantity = g.Sum(x => x.Quantity) })
        //                                         .OrderByDescending(product => product.Quantity)
        //                                         .Select(p => p.Name)
        //                                         .ToListAsync();

        //    return result;
        //}

        //public async Task<Dictionary<string, int?>> GetProductsQuantitiesWithProductNamesAsync()
        //{
        //    await context.DisposeAsync();
        //    context = new();
        //    var result = await context.ProductsQuantities.Include(x => x.Product)
        //                                                 .ToDictionaryAsync(g => g.Product.Name, g => g.ProductQuantity);

            
        //    //List<KeyValuePair<string, int?>> keyValuePairs = new();

        //    //foreach (var kvp in result)
        //    //{
        //    //    keyValuePairs.Add(new KeyValuePair<string, int?>(kvp.Key, kvp.Value));
        //    //}

        //    return result;
        //}

        private async Task<decimal> GetProductPrice(int productId)
        {
            await context.DisposeAsync();
            context = new();
            return context.Products.FirstOrDefaultAsync(p => p.Id == productId).Result.Price.Value;
        }

        public List<TransactionItemModel> GetTransactionItemsWithNames(int transactionId)
        {
            context.Dispose();
            context = new();
            var result = context.TransactionItems
                    .Where(t => t.TransactionId == transactionId)
                    .GroupJoin(context.Products,
                        _transaction => _transaction.ProductId,
                        _product => _product.Id,
                        (_transaction, _products) => new { _transaction, _products })
                    .SelectMany(x => x._products.DefaultIfEmpty(),
                        (x, _product) => new TransactionItemModel
                        (
                            _product != null ? _product.Id : -1, // replace defaultProductId with the default value you want to use
                            _product != null ? _product.Name : "Deleted product", // replace defaultProductName with the default value you want to use
                            x._transaction.Quantity.Value
                        )
                    )
                    .ToList();

            return result;
        }

        /// <summary>
        /// For transactions that can't be fully refunded, but partially
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public async Task<bool> IsTransactiontFullyRefundable(int transactionId)
        {
            var itemsFromTransaction = GetTransactionItemsWithNames(transactionId);

            foreach (var item in itemsFromTransaction)
            {
                if (item.ProductId == -1)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// For transactions that may contain items that are deleted
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public async Task<bool> IsTransactionPartiallyRefundable(int transactionId)
        {
            var itemsFromTransaction = GetTransactionItemsWithNames(transactionId);

            foreach (var item in itemsFromTransaction)
            {
                if (item.ProductId != -1)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Database Update

        public async Task UpdateExistingCategoryName(int categoryId, string newCategoryName)
        {
            await context.DisposeAsync();
            context = new();
            var result = context.Categories.SingleOrDefaultAsync(cid => cid.Id == categoryId);
            if (result != null)
            {
                result.Result.Name = newCategoryName;
                context.SaveChanges();
            }
        }

        public async Task UpdateExistingProduct(Product newProduct)
        {
            await context.DisposeAsync();
            context = new();
            var result = context.Products.SingleOrDefaultAsync(p => p.Id == newProduct.Id);
            if (result != null)
            {
                result.Result.Name = newProduct.Name;
                result.Result.Price = newProduct.Price;
                result.Result.CategoryId = newProduct.CategoryId;

                var quantityRow = context.ProductsQuantities.SingleOrDefaultAsync(p => p.ProductId == newProduct.Id);
                quantityRow.Result.ProductQuantity = newProduct.ProductsQuantities.FirstOrDefault().ProductQuantity;

                context.SaveChanges();
            }
        }

        private async Task UpdateProductQuantity(int productId, int quantity, char operation)
        {
            await context.DisposeAsync();
            context = new();

            var result = context.ProductsQuantities.Single(p => p.ProductId == productId);
            if(result != null)
            {
                switch (operation)
                {
                    case '-':
                        result.ProductQuantity -= quantity;
                        break;

                    case '+':
                        result.ProductQuantity += quantity;
                        Debug.WriteLine($" {productId} Quantity test: {quantity}");

                        break;
                }
               await context.SaveChangesAsync();
            }
            
        }

        public async Task UpdateTransactionItemQuantity(Transaction transaction, List<TransactionItemModel> transactionItems, bool isThereAnDeletedProduct)
        {
            await context.DisposeAsync();
            context = new();

            List<TransactionItem> oldTransactionItems = await GetTransactionItemsForSpecificTransaction(transaction.Id);
            
            decimal newPaymentValue = 0;

            foreach (var item in transactionItems)
            {
                var oldTransactionItem = oldTransactionItems.FirstOrDefault(oldItem => oldItem.ProductId == item.ProductId);

                if (oldTransactionItem != null)
                {
                    if (oldTransactionItem.Quantity.Value != item.ProductQuantity)
                    {
                        int difference = oldTransactionItem.Quantity.Value - item.ProductQuantity;
                        oldTransactionItem.Quantity = item.ProductQuantity;

                        //
                        await context.SaveChangesAsync();

                        await UpdateProductQuantity(item.ProductId, difference, '+');
                    }

                    newPaymentValue += await GetProductPrice(item.ProductId) * item.ProductQuantity;
                }
            }

            var result = await context.Transactions.SingleOrDefaultAsync(t => t.Id == transaction.Id);

            if (result != null)
            {
                if (isThereAnDeletedProduct)
                {
                    result.PaymentValue -= newPaymentValue;
                }
                else
                {
                    result.PaymentValue = newPaymentValue;
                }
            }

            await context.SaveChangesAsync();
        }
        #endregion

        #region Database Delete
        public async Task DeleteCategory(int categoryId)
        {
            await context.DisposeAsync();
            context = new();
            var categoryToRemove = await context.Categories.SingleOrDefaultAsync(cid => cid.Id == categoryId);
            if (categoryToRemove != null)
            {
                context.Categories.Remove(categoryToRemove);
                context.SaveChanges();
            }
        }
        
        public async Task DeleteProduct(int productId)
        {
            await context.DisposeAsync();
            context = new();
            var productToRemove = await context.Products.SingleOrDefaultAsync(pid => pid.Id == productId);
            if (productToRemove != null)
            {
                await DeleteProductFromProductQuantity(productId);
                context.Products.Remove(productToRemove);
                context.SaveChanges();
            }
        }

        public async Task DeleteTransactoion(int transationId)
        {
            await context.DisposeAsync();
            context = new();
            await DeleteItemsFromTransaction(transationId);
            var transactionToRemove = await context.Transactions.SingleOrDefaultAsync(pid => pid.Id == transationId);
            if (transactionToRemove != null)
            {
                context.Transactions.Remove(transactionToRemove);
                context.SaveChanges();
            }
        }



        private async Task DeleteItemsFromTransaction(int transationId)
        {
            await context.DisposeAsync();
            context = new();
            var itemsToDelete = await context.TransactionItems.Where(i => i.TransactionId == transationId).ToListAsync();
            foreach (var item in itemsToDelete)
            {
                UpdateProductQuantity(item.ProductId.Value, item.Quantity.Value, '+');
            }
            context.TransactionItems.RemoveRange(itemsToDelete);
            context.SaveChanges();
        }

        public async Task DeleteTranscationItems(int transactionId, List<TransactionItemModel> transactionItemForDeletion)
        {
            await context.DisposeAsync();
            context = new();

            var transactionItems = await context.TransactionItems.Where(t => t.TransactionId == transactionId).ToListAsync();
            List<TransactionItem> tmpTransationItems = new();

            //var transactionItems = await context.TransactionItems.Where(t => t.TransactionId == transactionId && transactionItemIdsList.Any(item => item.ProductId == t.ProductId)).ToListAsync();

            foreach (var item in transactionItems)
            {
                if (transactionItemForDeletion.Any(itemForDeletion => itemForDeletion.ProductId == item.ProductId))
                {
                    tmpTransationItems.Add(item);
                }
            }

            if (tmpTransationItems != null && tmpTransationItems.Count > 0)
            {
                foreach (var item in transactionItems)
                {
                    if (item.Quantity > 0 && item.Product != null)
                    {
                        await UpdateProductQuantity(item.ProductId.Value, item.Quantity.Value, '+');
                    }
                }

                context.TransactionItems.RemoveRange(tmpTransationItems);
                context.SaveChanges();
            }
        }

        public async Task DeleteProductsFromCategory(int categoryId)
        {
            await context.DisposeAsync();
            context = new();
            List<Product> productsToDelete = await context.Products.Where(i => i.CategoryId == categoryId).ToListAsync();

            if(productsToDelete != null)
            {
                await DeleteProductsFromProductQuantity(productsToDelete);
                context.Products.RemoveRange(productsToDelete);
                context.SaveChanges();
            }
        }

        private async Task DeleteProductFromProductQuantity(int productId)
        {
            await context.DisposeAsync();
            context = new();
            var productQuantity = await context.ProductsQuantities.FirstOrDefaultAsync(i => i.ProductId == productId);
            context.ProductsQuantities.Remove(productQuantity);
            await context.SaveChangesAsync();
        }

        private async Task DeleteProductsFromProductQuantity(List<Product> productsToDelet)
        {
            await context.DisposeAsync();
            context = new();
            var productIds = productsToDelet.Select(p => p.Id).ToList();
            var productQuantitisToRemove = await context.ProductsQuantities.Where(x => productIds.Contains(x.ProductId.Value)).ToListAsync();

            context.ProductsQuantities.RemoveRange(productQuantitisToRemove);
            await context.SaveChangesAsync();
        }

            #endregion

        public async Task TestDBFunctionality()
        {
            await context.DisposeAsync();
            context = new();
            var result = context.Products.Join(
                context.ProductsQuantities,
                _prod => _prod.Id,
                _prodq => _prodq.ProductId,
                (_prod, _prodq) => new
                {
                    _prod.Name,
                    _prodq.ProductQuantity
                }
                ).ToList();
            
            foreach ( var item in result )
            {
                Debug.WriteLine( item );
            }
        }
    }
}
