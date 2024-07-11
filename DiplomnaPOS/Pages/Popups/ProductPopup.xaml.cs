using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using EfLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiplomnaPOS.Pages.Popups;

public partial class ProductPopup : Popup, INotifyPropertyChanged
{
	public ObservableCollection<Category> categories { get; set; }

    public Product productPopup;

    //private bool _isFormValid;
    //public bool IsFormValid
    //{
    //    get { return _isFormValid; }
    //    set
    //    {
    //        if (_isFormValid != value)
    //        {
    //            _isFormValid = value;
    //            OnPropertyChanged();
    //        }
    //    }
    //}


    //public event PropertyChangedEventHandler PropertyChanged;

    //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    //{
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //}

    public ProductPopup(List<Category> _categoriesArray, int currentCategoryId)
    {
        InitializeComponent();
        popupName.Text = "Add product";
        this.categories = _categoriesArray.ToObservableCollection();
        productPopup = new();


        //TODO Find a better way to fill the picker with data
        foreach (var item in _categoriesArray)
        {
            CategoryPicker.Items.Add(item.Name);
        }

        if (currentCategoryId != null)
        {
            CategoryPicker.SelectedIndex = currentCategoryId - 1;
        }

        //IsFormValid = false;
        OkBtn.IsEnabled = false;

        nameValidator.PropertyChanged += InputFields_PropertyChanged;
        priceValidator.PropertyChanged += InputFields_PropertyChanged;
        quantityValidator.PropertyChanged += InputFields_PropertyChanged;
        CategoryPicker.PropertyChanged += InputFields_PropertyChanged;
    }

    public ProductPopup(List<Category> _categoriesArray, Product _product, int _productQuantity)
	{
		InitializeComponent();
        popupName.Text = "Edit product";
        this.categories = _categoriesArray.ToObservableCollection();
		this.productPopup = _product;

        //TODO Find a better way to fill the picker with data
        foreach (var item in _categoriesArray)
        {
            CategoryPicker.Items.Add(item.Name);
        }

        field1_data.Text = productPopup.Name;
        field2_data.Text = productPopup.Price.ToString();
        field3_data.Text = _productQuantity.ToString();

        //Ensure when editing a product, it is marked with the proper category
        CategoryPicker.SelectedIndex = CategoryPicker.Items.IndexOf(categories.FirstOrDefault(c => c.Id == productPopup.CategoryId).Name);

        nameValidator.PropertyChanged += InputFields_PropertyChanged;
        priceValidator.PropertyChanged += InputFields_PropertyChanged;
        quantityValidator.PropertyChanged += InputFields_PropertyChanged;
        CategoryPicker.PropertyChanged += InputFields_PropertyChanged;

    }

    private void OkBtn_Clicked(object sender, EventArgs e)
    {
        int selectedIndex = CategoryPicker.SelectedIndex;
        productPopup.CategoryId = categories.FirstOrDefault(c => c.Name == CategoryPicker.Items[selectedIndex]).Id;
        productPopup.Name = field1_data.Text;
        productPopup.Price = decimal.Parse(field2_data.Text);

        ProductsQuantity productsQuantity = new ProductsQuantity();
        productsQuantity.ProductId = productPopup.Id;
        productsQuantity.ProductQuantity = int.Parse(field3_data.Text);

        productPopup.ProductsQuantities.Add(productsQuantity);
		Close(productPopup);
    }

    private void CancelBtn_Clicked(object sender, EventArgs e)
    {
        Close();
    }

    private void InputFields_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if ((nameValidator.IsValid && nameValidator.Value != null) &&
            (priceValidator.IsValid && priceValidator.Value != null) &&
            (quantityValidator.IsValid && quantityValidator.Value != null) &&
            CategoryPicker.SelectedIndex >= 0)
        {
            //IsFormValid = true;
            OkBtn.IsEnabled = true;
        }
        else
        {
            OkBtn.IsEnabled = false;
        }
    }

}