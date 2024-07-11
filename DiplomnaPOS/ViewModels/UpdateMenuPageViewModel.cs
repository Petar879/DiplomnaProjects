using CommunityToolkit.Mvvm.ComponentModel;
using EfLibrary.Models;
using DiplomnaPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Views;
using DiplomnaPOS.Pages.Popups;

namespace DiplomnaPOS.Pages
{
    public partial class UpdateMenuPageViewModel : ObservableObject
    {
        private DatabaseManager _databaseManager = new();
        private List<Product> _productsList = new();

        private int _currentCategoryId;

        [ObservableProperty]
        private List<Category> _categoriesList = new();

        [ObservableProperty]
        private List<Product> _displayFilteredProducts = new();
        public UpdateMenuPageViewModel()
        {
        }

        public async Task LoadDataAsync()
        {
            CategoriesList = await _databaseManager.GetCategories();
            _productsList = await _databaseManager.GetProducts();
        }

        [RelayCommand]
        public async Task UpdateCategoryName(Category category)
        {
            string result = await Application.Current.MainPage.DisplayPromptAsync($"Update \"{category.Name}\" category", "New category name", initialValue: category.Name);
            if (result != null)
            {
                await _databaseManager.UpdateExistingCategoryName(category.Id, result);
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        public async Task DeleteCategoryName(Category category)
        {
            bool answer;

            if (category.Products.Count != 0)
            {
                answer = await Application.Current.MainPage.DisplayAlert("Alert!", $"Deleting category \"{category.Name}\" will also remove {category.Products.Count} associated product(s).\nProceed with the deletion?", "Yes", "No");
            }
            else
            {
                answer = await Application.Current.MainPage.DisplayAlert("Alert!", $"Proceed with the deleting \"{category.Name}\"?", "Yes", "No");
            }

            if (answer)
            {
                if (category.Products.Count != 0)
                {
                    await _databaseManager.DeleteProductsFromCategory(category.Id);
                }

                await _databaseManager.DeleteCategory(category.Id);
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        public async Task AddNewCategoryName()
        {
            string result = await Application.Current.MainPage.DisplayPromptAsync($"Add category", "New category name");
            if (result != null && result != string.Empty )
            {
                await _databaseManager.AddNewCategory(result);
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        public async Task FilterCurrentProducts(Category category)
        {
            DisplayFilteredProducts.Clear();
            DisplayFilteredProducts =  _productsList.Where(p => p.CategoryId == category.Id).ToList();
            _currentCategoryId = category.Id;
        }

        [RelayCommand]
        public async Task DeleteProduct(Product product)
        {
            bool answer = await Application.Current.MainPage.DisplayAlert("Alert!", $"Do you want to delete \"{product.Name}\"", "Yes", "No");
            if (answer)
            {
                await _databaseManager.DeleteProduct(product.Id);
                await LoadDataAsync();
                DisplayFilteredProducts = _productsList.Where(p => p.CategoryId == _currentCategoryId).ToList();
            }
        }

        [RelayCommand]
        public async Task AddNewProduct()
        {
            var result = await Application.Current.MainPage.ShowPopupAsync(new ProductPopup(CategoriesList, _currentCategoryId));
            
            if(result != null)
            {
                await _databaseManager.AddNewProduct(result as Product);
                await LoadDataAsync();
                DisplayFilteredProducts = _productsList.Where(p => p.CategoryId == _currentCategoryId).ToList();
            }
        }

        [RelayCommand]
        public async Task EditProduct(Product product)
        {
            var result = await Application.Current.MainPage.ShowPopupAsync(new ProductPopup(CategoriesList, product, await _databaseManager.GetSpecificProductQuantity(product.Id)));
            if (result != null)
            {
                await _databaseManager.UpdateExistingProduct(result as Product);
                await LoadDataAsync();
                DisplayFilteredProducts = _productsList.Where(p => p.CategoryId == _currentCategoryId).ToList();
            }
        }

    }
}
