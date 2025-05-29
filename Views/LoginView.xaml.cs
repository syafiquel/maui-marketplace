using NYC.MobileApp.ViewModel;

namespace NYC.MobileApp.Views;

public partial class LoginView : ContentPage
{
	public LoginView()
	{
		InitializeComponent();
        BindingContext = new LoginViewModel();
    }
}