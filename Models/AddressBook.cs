using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NYC.MobileApp.Models;

public class AddressBookModel : INotifyPropertyChanged
{
    private int _id;
    private string _name;
    private string _phone;
    private string _address1;
    private string _address2;
    private string _city;
    private string _postalCode;
    private string _state;
    private string _managedBy;
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public String ManagedBy
    {
        get => _managedBy;
        set => SetProperty(ref _managedBy, value);
    }

   

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }

    public string Address1
    {
        get => _address1;
        set => SetProperty(ref _address1, value);
    }

    public string Address2
    {
        get => _address2;
        set => SetProperty(ref _address2, value);
    }

    public string City
    {
        get => _city;
        set => SetProperty(ref _city, value);
    }

    public string PostalCode
    {
        get => _postalCode;
        set => SetProperty(ref _postalCode, value);
    }

    public string State
    {
        get => _state;
        set => SetProperty(ref _state, value);
    }

    public string FullAddress
    {
        get
        {
            var parts = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(Address1))
                parts.Add(Address1);

            if (!string.IsNullOrWhiteSpace(Address2))
                parts.Add(Address2);
            
            if (!string.IsNullOrWhiteSpace(PostalCode) && !string.IsNullOrWhiteSpace(City))
                parts.Add($"{PostalCode}, {City}");
            else if (!string.IsNullOrWhiteSpace(City))
                parts.Add(City);
            else if (!string.IsNullOrWhiteSpace(PostalCode))
                parts.Add(PostalCode);
            
            if (!string.IsNullOrWhiteSpace(State))
                parts.Add(State + ".");

            return string.Join(" ", parts);
        }
    }

    public AddressBookModel()
    {
        Id = 0;
        Name = string.Empty;
        Phone = string.Empty;
        Address1 = string.Empty;
        Address2 = string.Empty;
        City = string.Empty;
        PostalCode = string.Empty;
        State = string.Empty;
        ManagedBy = string.Empty;
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
        
        // Notify FullAddress when address components change
        if (propertyName is nameof(Address1) or nameof(Address2) or nameof(City) or nameof(PostalCode) or nameof(State))
        {
            OnPropertyChanged(nameof(FullAddress));
        }
        
        return true;
    }
}