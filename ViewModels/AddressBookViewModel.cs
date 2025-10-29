using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using NYC.MobileApp.Model;
using NYC.MobileApp.Models;
using NYC.MobileApp.Views;
using NYC.MobileApp.API;

namespace NYC.MobileApp.ViewModel;

public class AddressBookViewModel : INotifyPropertyChanged
{
    private ObservableCollection<AddressBookModel> _addresses;
    private ObservableCollection<AddressBookModel> _filteredAddresses;
    private string _searchText;
    private bool _isLoaded;
    private readonly IApiService _apiService;

    public ObservableCollection<AddressBookModel> Addresses
    {
        get => _addresses;
        set => SetProperty(ref _addresses, value);
    }

    public ObservableCollection<AddressBookModel> FilteredAddresses
    {
        get => _filteredAddresses;
        set => SetProperty(ref _filteredAddresses, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            FilterAddresses();
        }
    }

    public bool IsLoaded
    {
        get => _isLoaded;
        set => SetProperty(ref _isLoaded, value);
    }

    public ICommand AddAddressCommand { get; }
    public ICommand EditAddressCommand { get; }
    public ICommand DeleteAddressCommand { get; }

    public AddressBookViewModel()
    {
        Addresses = new ObservableCollection<AddressBookModel>();
        FilteredAddresses = new ObservableCollection<AddressBookModel>();
        SearchText = string.Empty;

        AddAddressCommand = new Command(async () => await AddAddress());
        EditAddressCommand = new Command<AddressBookModel>(async (address) => await EditAddress(address));
        DeleteAddressCommand = new Command<AddressBookModel>(async (address) => await DeleteAddress(address));

        var serviceProvider = MauiApplication.Current.Services;
        _apiService = ActivatorUtilities.GetServiceOrCreateInstance<ApiService>(serviceProvider);

        LoadSampleData();
    }

    private async void LoadSampleData()
    {
        IsLoaded = false;
        var addressBooks = await _apiService.GetAsync<PaginatedList<AddressBookModel>>("addressbooks?offset=1&limit=100");
        Addresses = new ObservableCollection<AddressBookModel>(addressBooks.Items);
        FilterAddresses();
        IsLoaded = true;
    }

    private void FilterAddresses()
    {
        FilteredAddresses.Clear();

        var filtered = string.IsNullOrWhiteSpace(SearchText) 
            ? Addresses 
            : Addresses.Where(a => 
                a.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                a.Phone.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                a.FullAddress.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        foreach (var address in filtered)
        {
            FilteredAddresses.Add(address);
        }
    }

    private async Task AddAddress()
    {
        try
        {
            await Shell.Current.GoToAsync(nameof(EditAddressView));
            
            // Refresh the list when returning
            MessagingCenter.Subscribe<EditAddressViewModel, AddressBookModel>(this, "AddressAdded", async (sender, address) =>
            {
                await _apiService.PostAsync<AddressBookModel>("addressbook", address);
                Addresses.Add(address);
                FilterAddresses();
                MessagingCenter.Unsubscribe<EditAddressViewModel, AddressBookModel>(this, "AddressAdded");
                Application.Current.MainPage.DisplayAlert("Info", "Adress book has been created", "OK");
            });
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Unable to add address: {ex.Message}", "OK");
        }
    }

    private async Task EditAddress(AddressBookModel address)
    {
        try
        {
            var editPage = new EditAddressView(address);
            await Shell.Current.Navigation.PushAsync(editPage);

            // Refresh the list when returning
            MessagingCenter.Subscribe<EditAddressViewModel, AddressBookModel>(this, "AddressUpdated", async (sender, updatedAddress) =>
            {
                var index = Addresses.IndexOf(address);
                if (index >= 0)
                {
                    await _apiService.PutAsync<AddressBookModel>("addressbook", updatedAddress);
                    Addresses[index] = updatedAddress;
                    FilterAddresses();
                }
                MessagingCenter.Unsubscribe<EditAddressViewModel, AddressBookModel>(this, "AddressUpdated");
                Application.Current.MainPage.DisplayAlert("Info", "Adress book has been updated", "OK");
            });
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Unable to edit address: {ex.Message}", "OK");
        }
    }

    private async Task DeleteAddress(AddressBookModel address)
    {
        try
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Delete Address", 
                $"Are you sure you want to delete {address.Name} with ID {address.Id}?", 
                "Yes", "No");

            if (confirm)
            {
                var url = $"addressbook/{address.Id}";
                await _apiService.DeleteAsync(url);
                Addresses.Remove(address);
                FilterAddresses();
                Application.Current.MainPage.DisplayAlert("Info", "Adress book has been deleted", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Unable to delete address: {ex.Message}", "OK");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}