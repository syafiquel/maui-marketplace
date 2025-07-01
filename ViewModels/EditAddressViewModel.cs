using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using NYC.MobileApp.Models;

namespace NYC.MobileApp.ViewModel;

public class EditAddressViewModel : INotifyPropertyChanged
{
    private AddressBookModel _address;
    private bool _isEditMode;

    public AddressBookModel Address
    {
        get => _address;
        set => SetProperty(ref _address, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public string PageTitle => IsEditMode ? "Edit Address" : "Add Address";

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public EditAddressViewModel()
    {
        Address = new AddressBookModel();
        IsEditMode = false;
        
        SaveCommand = new Command(async () => await SaveAddress());
        CancelCommand = new Command(async () => await Cancel());
    }

    public EditAddressViewModel(AddressBookModel address)
    {
        // Create a copy for editing to avoid modifying the original until save
        Address = new AddressBookModel
        {
            Id = address.Id,
            Name = address.Name,
            Phone = address.Phone,
            Address1 = address.Address1,
            Address2 = address.Address2,
            City = address.City,
            PostalCode = address.PostalCode,
            State = address.State
        };
        IsEditMode = true;
        
        SaveCommand = new Command(async () => await SaveAddress());
        CancelCommand = new Command(async () => await Cancel());
    }

    private async Task SaveAddress()
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(Address.Name))
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "Name is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Address.Phone))
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "Phone is required.", "OK");
                return;
            }

            // Send message to parent view model
            if (IsEditMode)
            {
                MessagingCenter.Send(this, "AddressUpdated", Address);
            }
            else
            {
                MessagingCenter.Send(this, "AddressAdded", Address);
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Unable to save address: {ex.Message}", "OK");
        }
    }

    private async Task Cancel()
    {
        try
        {
            bool hasChanges = HasUnsavedChanges();
            
            if (hasChanges)
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Unsaved Changes", 
                    "You have unsaved changes. Are you sure you want to cancel?", 
                    "Yes", "No");

                if (!confirm)
                    return;
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Unable to cancel: {ex.Message}", "OK");
        }
    }

    private bool HasUnsavedChanges()
    {
        // For new addresses, check if any field has data
        if (!IsEditMode)
        {
            return !string.IsNullOrWhiteSpace(Address.Name) ||
                   !string.IsNullOrWhiteSpace(Address.Phone) ||
                   !string.IsNullOrWhiteSpace(Address.Address1) ||
                   !string.IsNullOrWhiteSpace(Address.Address2) ||
                   !string.IsNullOrWhiteSpace(Address.City) ||
                   !string.IsNullOrWhiteSpace(Address.PostalCode) ||
                   !string.IsNullOrWhiteSpace(Address.State);
        }

        // For edit mode, this would require storing the original values
        // For simplicity, always show confirmation in edit mode
        return true;
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