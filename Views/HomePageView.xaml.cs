using NYC.MobileApp.ViewModel;

namespace NYC.MobileApp.Views;

public partial class HomePageView : ContentPage
{
    public HomePageView()
    {
        InitializeComponent();
        BindingContext = new HomePageViewModel();
    }

    private async void ItemsView_OnRemainingItemsThresholdReached(object? sender, EventArgs e)
    {
        var vm = BindingContext as HomePageViewModel;
        await vm.PopulateBestSellingAsync();
    }
}