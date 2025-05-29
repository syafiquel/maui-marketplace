
using NYC.MobileApp.Views;
using System.Windows.Input;
using NYC.MobileApp.API;

namespace NYC.MobileApp.ViewModel
{
    public class LoginViewModel: BaseViewModel
    {
        private readonly IApiService _apiService;
        
        private string _Email;
        public string Email
        {
            get => _Email;
            set => SetProperty(ref _Email, value);
        }

        private string _Password;
        public string Password
        {
            get => _Password;
            set => SetProperty(ref _Password, value);
        }
        
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public LoginViewModel()
        {
            LoginCommand = new Command(async() => await LoginAsync());
            ForgotPasswordCommand = new Command(ForgotPassword);
            
            var serviceProvider = MauiApplication.Current.Services;
            _apiService = ActivatorUtilities.GetServiceOrCreateInstance<ApiService>(serviceProvider);
        }
        
        private void ForgotPassword(object obj)
        {
           
        }

        private async Task LoginAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email and password are required";
                return;
            }
            
            try
            {
                var loginResponse = await _apiService.LoginAsync(Email, Password);
                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.AccessToken))
                {
                    Application.Current.MainPage = new AppShell();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Invalid email and password combination.";
            }
        }
    }
}
