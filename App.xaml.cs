using NYC.MobileApp.Views;

namespace NYC.MobileApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		if (Preferences.ContainsKey("BearerToken") && Preferences.ContainsKey("TokenExpiresAt"))
		{
			string token = Preferences.Get("BearerToken", "");
			string expiresAtString = Preferences.Get("TokenExpiresAt", "");

			if (!string.IsNullOrEmpty(token) && DateTime.TryParse(expiresAtString, out DateTime expiresAt))
			{
				if (DateTime.UtcNow < expiresAt)
				{
					// Token is still valid, navigate to main page
					MainPage = new AppShell();
					return;
				}
			}
		}
		
		MainPage = new LoginView();
	}
}
