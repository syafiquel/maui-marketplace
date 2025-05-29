using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using NYC.MobileApp.Model;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using NYC.MobileApp.API;

namespace NYC.MobileApp.ViewModel;

[QueryProperty(nameof(ItemType), nameof(ItemType))]
public partial class ProductDetailsViewModel : ObservableObject
{
    double lastScrollIndex;
    double currentScrollIndex;
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string itemType;

    [ObservableProperty]
    private bool isFooterVisible;

    [ObservableProperty]
    private bool isFavorite;

    public Color FavStatusColor => IsFavorite ? Color.FromArgb("#00C569") : Color.FromArgb("#000000");

    [ObservableProperty]
    private ProductDetail productDetail = new();

    [ObservableProperty]
    private bool isLoaded;

    [ObservableProperty]
    private decimal cartTotalPrice;

    public ICommand BackCommand { get; }
    public ICommand FavCommand { get; }

    public ICommand IncreaseQuantityCommand => new Command<ItemModel>(item => item.Quantity += 1);
    public ICommand DecreaseQuantityCommand => new Command<ItemModel>(item =>
    {
        if (item.Quantity > 0)
            item.Quantity -= 1;
    });

    public ProductDetailsViewModel()
    {
        BackCommand = new Command<object>(GoBack);
        FavCommand = new Command<Color>(FavItem);

        var serviceProvider = MauiApplication.Current.Services;
        _apiService = (IApiService)ActivatorUtilities.CreateInstance(serviceProvider, typeof(ApiService));
    }

    private async void GoBack(object obj)
    {
        await Application.Current.MainPage.Navigation.PopModalAsync();
    }

    private void FavItem(Color obj)
    {
        IsFavorite = !IsFavorite;
        OnPropertyChanged(nameof(FavStatusColor));
    }

    public void ChageFooterVisibility(double currentY)
    {
        currentScrollIndex = currentY;
        IsFooterVisible = currentScrollIndex <= lastScrollIndex;
        lastScrollIndex = currentScrollIndex;
    }

    partial void OnItemTypeChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            _ = PopulateDataAsync(value);
        }
    }

    partial void OnProductDetailChanged(ProductDetail oldValue, ProductDetail newValue)
    {
        SubscribeToQuantityChanges();
        CalculateCartTotal();
    }

    private async Task PopulateDataAsync(string productCode)
    {
        ProductDetail = await _apiService.GetAsync<ProductDetail>($"itemtypes/{productCode}");
        ProductDetail.ItemType = productCode;
        IsLoaded = true;
    }

    private void SubscribeToQuantityChanges()
    {
        if (ProductDetail?.ColorGroups == null)
            return;

        foreach (var item in ProductDetail.ColorGroups.SelectMany(cg => cg.Items))
        {
            item.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ItemModel.Quantity))
                    CalculateCartTotal();
            };
        }
    }

    private void CalculateCartTotal()
    {
        if (ProductDetail?.ColorGroups == null)
        {
            CartTotalPrice = 0;
            return;
        }

        CartTotalPrice = ProductDetail.ColorGroups
            .SelectMany(cg => cg.Items)
            .Sum(item => item.TotalPrice);
    }
    
    private ShoppingCartModel BuildShoppingCart()
    {
        var selectedItems = ProductDetail.ColorGroups
            .SelectMany(cg => cg.Items.Select(item => new ShoppingCartItemModel
            {
                ItemCode = item.ItemCode,
                Size = item.Size,
                Color = cg.Color,
                UOM = item.ItemUom.UOM,
                Price = item.ItemUom.Price,
                Quantity = item.Quantity
            }))
            .Where(item => item.Quantity > 0)
            .ToList();

        return new ShoppingCartModel
        {
            ItemType = ProductDetail.ItemType,
            Items = selectedItems
        };
    }
    
    public IRelayCommand AddToCartCommand => new RelayCommand(async () => await AddToCartAsync());

    private async Task AddToCartAsync()
    {
        if (ProductDetail?.ColorGroups == null)
            return;

        var selectedItems = ProductDetail.ColorGroups
            .SelectMany(cg => cg.Items)
            .Where(i => i.Quantity > 0)
            .ToList();

        if (!selectedItems.Any())
        {
            // Optional: alert user nothing was selected
            await Application.Current.MainPage.DisplayAlert("Cart", "Please select at least one item.", "OK");
            return;
        }

        var cartPayload = new
        {
            ItemType = ProductDetail.ItemType,
            
            Items = selectedItems.Select(item => new
            {
                ItemCode = item.ItemCode,
                Quantity = item.Quantity,
                UOM = item.ItemUom.UOM,
                Price = item.ItemUom.Price
            }).ToList()
        };
        
        
        var apiResponse = await _apiService.PostAsync<ApiResponse<ShoppingCartModel>>("cart", cartPayload);
        ToastDuration duration = ToastDuration.Short;
        double fontSize = 14;

        var toast = Toast.Make(apiResponse.Message, duration, fontSize);
        await toast.Show();
    }
}
