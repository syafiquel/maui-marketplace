using NYC.MobileApp.Model;
using NYC.MobileApp.Views;
using NYC.MobileApp.API;
using System.Windows.Input;

namespace NYC.MobileApp.ViewModel
{
    public class ProfileViewModel : BaseViewModel
    {        
        private readonly IApiService _apiService;
        public string ImageUrl { get; set; } = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS3CdlJjPVJk4he7HLKRgpQuKvomY4eGQ2Yxg&s";

        private List<MenuItems> _MenuItems = [];
        public List<MenuItems> MenuItems
        {
            get => _MenuItems;
            set => SetProperty(ref _MenuItems, value);
        }

        private bool _IsLoaded;
        public bool IsLoaded
        {
            get => _IsLoaded;
            set => SetProperty(ref _IsLoaded, value);
        }

        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string email;
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public ICommand SelectMenuCommand { get; private set; }
        public ProfileViewModel()
        {
            SelectMenuCommand = new Command<MenuItems>(SelectMenu);
            var serviceProvider = MauiApplication.Current.Services;
            _apiService = ActivatorUtilities.GetServiceOrCreateInstance<ApiService>(serviceProvider);
            _ = InitializeAsync();
        }
        private async Task InitializeAsync()
        {
            Name = Preferences.Get("UserFullName", "Test User");
            Email = Preferences.Get("UserEmail", "Test Email");
            await PopulateDataAsync();
        }
        async Task PopulateDataAsync()
        {
            await Task.Delay(500);
            //TODO: Remove Delay here and call API if needed
            MenuItems.Clear();
            MenuItems.Add(new MenuItems() { Title = "Edit Profile", Body = "\uf3eb", TargetType = typeof(UserProfileView) });
            //MenuItems.Add(new MenuItems() { Title = "Address Book", Body = "\uf3eb", TargetType = typeof(AddressBookView) });
            MenuItems.Add(new MenuItems() { Title = "Shipping Address", Body = "\uf34e", TargetType = typeof(ShippingAddressView) });
            MenuItems.Add(new MenuItems() { Title = "Wishlist", Body = "\uf2d5", TargetType = typeof(WishListView) });
            //MenuItems.Add(new MenuItems() { Title = "Order History", Body = "\uf150", TargetType = typeof(OrderDetailsView) });
            MenuItems.Add(new MenuItems() { Title = "Track Order", Body = "\uf787", TargetType = typeof(OrderDetailsView) });
            MenuItems.Add(new MenuItems() { Title = "Cards", Body = "\uf19b", TargetType = typeof(CardView) });
            MenuItems.Add(new MenuItems() { Title = "Notifications", Body = "\uf09c"});
            IsLoaded = true;
        }

        private async void SelectMenu(MenuItems item)
        {
            if (item.TargetType != null)
            {
                // if (item.TargetType == typeof(LoginView))
                // {
                //     var response = await App.Current.MainPage.DisplayAlert("Logout", "Do you want to logout?", "Yes", "No");
                //     if (response)
                //         try
                //         {
                //             await _apiService.LogoutUserAsync();
                //             App.Current.MainPage = new LoginView();
                //         }
                //         catch (Exception ex)
                //         {
                //             await Application.Current.MainPage.DisplayAlert("Error", "Logout failed", "OK");
                //         }
                // }
                // else
                // {
                //     await Application.Current.MainPage.Navigation.PushAsync(((Page)Activator.CreateInstance(item.TargetType)));
                // }
                
                await Application.Current.MainPage.Navigation.PushAsync(((Page)Activator.CreateInstance(item.TargetType)));
            }
            
        }       
    }
}
