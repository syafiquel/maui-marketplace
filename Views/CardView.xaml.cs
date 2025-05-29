using NYC.MobileApp.ViewModel;

namespace NYC.MobileApp.Views;

public partial class CardView : ContentPage
{
	public CardView()
	{
		InitializeComponent();
		BindingContext = new CardViewModel();

    }
}