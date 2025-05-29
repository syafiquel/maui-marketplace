using NYC.MobileApp.ViewModel;

namespace NYC.MobileApp.Views;

public partial class ProfileView : ContentPage
{
    public ProfileView()
    {
        InitializeComponent();
        BindingContext = new ProfileViewModel();
    }
}