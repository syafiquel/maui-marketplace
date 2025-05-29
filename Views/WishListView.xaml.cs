using NYC.MobileApp.ViewModel;
namespace NYC.MobileApp.Views;

public partial class WishListView : ContentPage
{   
    public WishListView()
	{
		InitializeComponent();
		BindingContext = new WishListViewModel();

    }
}