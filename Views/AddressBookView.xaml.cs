using NYC.MobileApp.ViewModel;

namespace NYC.MobileApp.Views;

public partial class AddressBookView : ContentPage
{
    public AddressBookView()
    {
        InitializeComponent();
        BindingContext = new AddressBookViewModel();
    }
}