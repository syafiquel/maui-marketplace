using NYC.MobileApp.ViewModel;
using NYC.MobileApp.API;

namespace NYC.MobileApp.Views;

public partial class UserProfileView : ContentPage
{
    private readonly IApiService _apiService;
    public UserProfileView()
    {
        InitializeComponent();
        var serviceProvider = MauiApplication.Current.Services;
        _apiService = ActivatorUtilities.GetServiceOrCreateInstance<ApiService>(serviceProvider);
    }

    private async void OnAddressBookTapped(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new AddressBookView());
    }

    private async void OnOrderHistoryTapped(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new OrderDetailsView());
    }

    private async void OnLogoutTapped(object sender, EventArgs e)
    {
        var response = await App.Current.MainPage.DisplayAlert("Logout", "Do you want to logout?", "Yes", "No");
        if (response)
        {
            try
            {
                await _apiService.LogoutUserAsync();
                App.Current.MainPage = new LoginView();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Logout failed", "OK");
            }
        }
    }

    private async void OnChangePasswordTapped(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new ChangePasswordView());
    }
}