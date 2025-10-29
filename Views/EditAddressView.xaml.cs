using NYC.MobileApp.Models;
using NYC.MobileApp.ViewModel;

namespace NYC.MobileApp.Views;

public partial class EditAddressView : ContentPage
{
    public EditAddressView()
    {
        InitializeComponent();
        BindingContext = new EditAddressViewModel();
    }

    public EditAddressView(AddressBookModel address)
    {
        InitializeComponent();
        BindingContext = new EditAddressViewModel(address);
    }
}