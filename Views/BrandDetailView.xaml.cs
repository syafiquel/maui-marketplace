using NYC.MobileApp.ViewModel;

namespace NYC.MobileApp.Views;

public partial class BrandDetailView : ContentPage
{
    public BrandDetailView()
    {
        InitializeComponent();
        BindingContext = new BrandDetailViewModel();
    }
}