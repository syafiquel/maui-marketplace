using NYC.MobileApp.Views;

namespace NYC.MobileApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(ProductDetailsView), typeof(ProductDetailsView));
		Routing.RegisterRoute(nameof(EditAddressView), typeof(EditAddressView));
	}
}
