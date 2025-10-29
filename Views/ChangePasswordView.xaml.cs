using NYC.MobileApp.API;

namespace NYC.MobileApp.Views;
using NYC.MobileApp.API;

public partial class ChangePasswordView : ContentPage
{
    private readonly IApiService _apiService;

    public ChangePasswordView()
    {
        InitializeComponent();
        var serviceProvider = MauiApplication.Current.Services;
        _apiService = ActivatorUtilities.GetServiceOrCreateInstance<ApiService>(serviceProvider);
    }

    private async void OnSubmitChangePassword(object sender, EventArgs e)
    {
        var currentPassword = CurrentPasswordEntry.Text;
        var newPassword = NewPasswordEntry.Text;
        var confirmPassword = ConfirmPasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(currentPassword) ||
            string.IsNullOrWhiteSpace(newPassword) ||
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            await DisplayAlert("Error", "All fields are required.", "OK");
            return;
        }

        if (newPassword != confirmPassword)
        {
            await DisplayAlert("Error", "New password and confirmation do not match.", "OK");
            return;
        }

        var confirm = await DisplayAlert("Confirm", "Change your password?", "Yes", "No");
        if (!confirm) return;

        try
        {
            var result = await _apiService.ChangePasswordAsync(currentPassword, newPassword, confirmPassword);
            if (result)
            {
                await DisplayAlert("Success", "Password changed successfully.", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to change password.", "OK");
        }
    }

    private async void OnCancelChangePassword(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}