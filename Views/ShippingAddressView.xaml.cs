using NYC.MobileApp.ViewModel;

namespace NYC.MobileApp.Views;

public partial class ShippingAddressView : ContentPage
{
	public ShippingAddressView()
	{
		InitializeComponent();
        BindingContext = new ShippingAddressViewModel();

    }
}