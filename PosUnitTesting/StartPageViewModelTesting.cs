using DiplomnaPOS.Models;
using DiplomnaPOS.Pages;
using EfLibrary.Models;
using System.Diagnostics;
using System.Xml.Linq;
using Xunit.Abstractions;

namespace PosUnitTesting
{
    public class StartPageViewModelTesting : IDisposable
    {
        readonly ITestOutputHelper _output;
        private StartPageViewModel _viewModel;

        
        public StartPageViewModelTesting(ITestOutputHelper outputHelper)
        {
            _output = outputHelper;
            _viewModel = new();
            InitializeCategoriesInViewModel();
            InitializeProductsInViewModel();
            InitializeProductQuantitiesInViewModel();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async void ProductFilteringTest(int inputCategoryId)
        {
            // Arrange
            _viewModel.CategoryId = inputCategoryId;

            // Act
            await _viewModel.UpdateFilteredCollection();
            bool AreItemsOfTheCategoryPresent = _viewModel.ProductsAfterFiltering.All(product => product.Product.CategoryId == inputCategoryId);

            // Assert
            Assert.True(AreItemsOfTheCategoryPresent);
        }

        [Fact]
        public async void ProductFilteringWithAMissingCategoryTest()
        {
            // Arrange
            _viewModel.CategoryId = 100;

            // Act
            await _viewModel.UpdateFilteredCollection();

            // Assert
            Assert.Empty(_viewModel.ProductsAfterFiltering);
        }

        [Theory]
        [InlineData(101, "CustomProduct", 0.5, 0, 0, 23)]
        [InlineData(10, "AnotherProduct", 33, 3, 22, 30)]
        [InlineData(44, "a", 7, 1, 3,3)]
        public async void AddingProductsToCartTest(int productId, string productName, decimal productPrice, int productCategoryId, int productCount, int productQuantityInventory)
        {
            // Arrange
            EfLibrary.Models.Product product = new EfLibrary.Models.Product()
            {
                Id = productId,
                Name = productName,
                Price = productPrice,
                CategoryId = productCategoryId,
            };
            CartProductModel cartProduct = new CartProductModel(product, productCount, (double)productPrice, productQuantityInventory);


            // Act
            await _viewModel.AddProductToCart(cartProduct);

            _output.WriteLine(productId.ToString());
            _output.WriteLine(productCount.ToString());
            _output.WriteLine(productQuantityInventory.ToString());
            bool isProductAddedToCart = _viewModel.ProductCart.ProductsCollection
                                        .Any(productInCollection => productInCollection.Product.Id == productId);

            // Assert
            Assert.True(isProductAddedToCart);

        }


        private void InitializeCategoriesInViewModel()
        {
            _viewModel.CategoryList = new()
            { 
                new EfLibrary.Models.Category()
                {
                    Id = 0,
                    Name = "TestCategory1",
                },
                new EfLibrary.Models.Category()
                {
                    Id = 1,
                    Name = "TestCategory2",
                },
                new EfLibrary.Models.Category()
                {
                    Id = 2,
                    Name = "TestCategory3",
                }
            };
        }

        private void InitializeProductsInViewModel()
        {
            _viewModel.productsFromDb = new()
            {
                new EfLibrary.Models.Product()
                {
                    Id = 0,
                    Name = "Product1",
                    Price = 10,
                    CategoryId = 0,
                },
                new EfLibrary.Models.Product()
                {
                    Id = 1,
                    Name = "Product2",
                    Price = 50,
                    CategoryId = 0,
                },
                new EfLibrary.Models.Product()
                {
                    Id = 2,
                    Name = "Product3",
                    Price = 1,
                    CategoryId = 0,
                },
                new EfLibrary.Models.Product()
                {
                    Id = 3,
                    Name = "Product4",
                    Price = 10,
                    CategoryId = 1,
                },
            };
        }

        private void InitializeProductQuantitiesInViewModel()
        {
            _viewModel.productsQuantityFromDb = new()
            {
                new EfLibrary.Models.ProductsQuantity()
                {
                    QuantityId = 0,
                    ProductId = 0,
                    ProductQuantity = 100,
                },
                new EfLibrary.Models.ProductsQuantity()
                {
                    QuantityId = 1,
                    ProductId = 1,
                    ProductQuantity = 5,
                },
                new EfLibrary.Models.ProductsQuantity()
                {
                    QuantityId = 2,
                    ProductId = 2,
                    ProductQuantity = 30,
                },
                new EfLibrary.Models.ProductsQuantity()
                {
                    QuantityId = 3,
                    ProductId = 3,
                    ProductQuantity = 0,
                }
            };
        }

    }
}