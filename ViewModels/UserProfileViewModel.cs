using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace NYC.MobileApp.ViewModel;

public partial class UserProfileViewModel : ObservableObject
{
    [ObservableProperty]
    private string name = "TF Manager";

    [ObservableProperty]
    private string company = "Tong Fan Sdn. Bhd";

    [ObservableProperty]
    private string role = "Counter";

    public ICommand NavigateToAddressBookCommand { get; }
    public ICommand NavigateToOrderHistoryCommand { get; }

    public UserProfileViewModel()
    {
        NavigateToAddressBookCommand = new AsyncRelayCommand(NavigateToAddressBook);
        NavigateToOrderHistoryCommand = new AsyncRelayCommand(NavigateToOrderHistory);
    }

    private async Task NavigateToAddressBook()
    {
        await Shell.Current.GoToAsync("AddressBookView");
    }

    private async Task NavigateToOrderHistory()
    {
        await Shell.Current.GoToAsync("OrderDetailsView");
    }
}