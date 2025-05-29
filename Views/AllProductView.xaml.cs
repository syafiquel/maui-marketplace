using NYC.MobileApp.ViewModel;
namespace NYC.MobileApp.Views;
public partial class AllProductView : ContentPage
{
    public AllProductView()
    {
        InitializeComponent();
        BindingContext = new AllProductViewModel();
    }
}