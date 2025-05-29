using NYC.MobileApp.ViewModel;

namespace NYC.MobileApp.Views;

public partial class ProductDetailsView : ContentPage
{
    public ProductDetailsView()
    {
        InitializeComponent();
    }
    
    private void ScrollViewScrolled(object sender, ScrolledEventArgs e)
    {
        var viewModel = BindingContext as ProductDetailsViewModel;
        viewModel.ChageFooterVisibility(e.ScrollY);
    }
}