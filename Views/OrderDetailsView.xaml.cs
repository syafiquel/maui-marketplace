using NYC.MobileApp.ViewModel;

namespace NYC.MobileApp.Views;

public partial class OrderDetailsView : ContentPage
{
    public OrderDetailsView()
    {
        InitializeComponent();
        BindingContext = new OrderDetailsViewModel();
    }
}