
using NYC.MobileApp.Model;
using NYC.MobileApp.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Android.Provider;
using NYC.MobileApp.API;

namespace NYC.MobileApp.ViewModel
{
    public class HomePageViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private int _pageNumber = 1;
        private bool _hasMoreItems = true; 
        private DateTime LastLoadMoreTimeStamp { get; set; } = DateTime.UtcNow;
        
        public int PageSize { get; } = 20;
        
        private ObservableCollection<CategoryModel> _CategoriesDataList = [];
        public ObservableCollection<CategoryModel> CategoriesDataList
        {
            get => _CategoriesDataList;
            set => SetProperty(ref _CategoriesDataList, value);

        }

        private ObservableCollection<ProductListModel> _BestSellingDataList = new ObservableCollection<ProductListModel>();
        public ObservableCollection<ProductListModel> BestSellingDataList
        {
            get => _BestSellingDataList;
            set => SetProperty(ref _BestSellingDataList, value);
        }

        private ObservableCollection<ProductListModel> _FeaturedBrandsDataList = [];
        public ObservableCollection<ProductListModel> FeaturedBrandsDataList
        {
            get => _FeaturedBrandsDataList;
            set => SetProperty(ref _FeaturedBrandsDataList, value);
        }

        private bool _isLoading = true;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        public ICommand SelectProductCommand { get; }
        public ICommand BrandTapCommand { get; }
        public ICommand RecommendedTapCommand { get; }
        public ICommand CategoryTapCommand { get; }
        public ICommand LoadMoreItemsCommand { get; }
        
        
        public HomePageViewModel()
        {
            var serviceProvider = MauiApplication.Current.Services;
            _apiService = (IApiService)ActivatorUtilities.CreateInstance(serviceProvider, typeof(ApiService));
            
            SelectProductCommand = new Command<ProductListModel>(SelectProduct);
            RecommendedTapCommand = new Command<object>(SelectRecommend);
            CategoryTapCommand = new Command<CategoryModel>(SelectCategory);
            BrandTapCommand = new Command<ProductListModel>(SelectBrand);
            LoadMoreItemsCommand = new Command(async() => await LoadMoreItemsAsync());
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await PopulateDataAsync(); 
        }
        async Task PopulateDataAsync()
        {
            var categoriesModel = await _apiService.PostAsync<PaginatedList<CategoryModel>>("itemGroups", new {});
            CategoriesDataList = new ObservableCollection<CategoryModel>(categoriesModel.Items);
            
            FeaturedBrandsDataList.Add(new ProductListModel() { Description = "B&o", Details = "5693 Products", ImageUrl = "https://raw.githubusercontent.com/exendahal/ecommerceXF/master/eCommerce/eCommerce.Android/Resources/drawable/Icon_Bo.png" });
            FeaturedBrandsDataList.Add(new ProductListModel() { Description = "Beats", Details = "1124 Products", ImageUrl = "https://raw.githubusercontent.com/exendahal/ecommerceXF/master/eCommerce/eCommerce.Android/Resources/drawable/beats.png" });
            FeaturedBrandsDataList.Add(new ProductListModel() { Description = "Apple Inc", Details = "5693 Products", ImageUrl = "https://raw.githubusercontent.com/exendahal/ecommerceXF/master/eCommerce/eCommerce.Android/Resources/drawable/Icon_Apple.png" });

            IsLoading = false;
            
            await Task.Delay(200);
            await PopulateBestSellingAsync();
        }

        private async Task LoadMoreItemsAsync()
        {
            if (DateTime.UtcNow >= LastLoadMoreTimeStamp.AddSeconds(5))
            {
                LastLoadMoreTimeStamp = DateTime.UtcNow;
                await PopulateBestSellingAsync();
            }
        }

        public async Task PopulateBestSellingAsync()
        {
            if (IsLoading || !_hasMoreItems)
                return;
            
            IsLoading = true;
            
            var recommendedProductsModel = await _apiService.PostAsync<PaginatedList<ProductListModel>>("itemtypes", new
            {
                Offset = _pageNumber,
                Limit = PageSize
            });

            _hasMoreItems = recommendedProductsModel.Items.Any();

            foreach (var product in recommendedProductsModel.Items)
            {
                BestSellingDataList.Add(product);
            }
            
            _pageNumber++;
            
            IsLoading = false;
        }

        private async void SelectBrand(ProductListModel obj)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new BrandDetailView());
        }
        
        private async void SelectProduct(ProductListModel productListModel)
        {
            await Shell.Current.GoToAsync($"{nameof(ProductDetailsView)}?{nameof(ProductListModel.ItemType)}={productListModel.ItemType}");
        }

        private async void SelectCategory(CategoryModel obj)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new CategoryDetailView(obj));
        }
        private async void SelectRecommend(object obj)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new AllProductView());
        }
    }
}
