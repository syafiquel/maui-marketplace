using System;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Android.Content.Res;
using NYC.MobileApp.API;
using NYC.MobileApp.ViewModel;
using NYC.MobileApp.Views;

namespace NYC.MobileApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseSentry(options => {
				options.Dsn = "https://d244b924bad14429eddb558337185623@o4507565044400128.ingest.us.sentry.io/4507565046300672";

				// Use debug mode if you want to see what the SDK is doing.
				// Debug messages are written to stdout with Console.Writeline,
				// and are viewable in your IDE's debug console or with 'adb logcat', etc.
				// This option is not recommended when deploying your application.
				options.Debug = true;

				// Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
				// We recommend adjusting this value in production.
				options.TracesSampleRate = 1.0;
			})

			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("Material-Icon.ttf", "MaterialIcon");
			})
			
			.Services.AddHttpClient<IApiService, ApiService>(client =>
			{
				client.Timeout = TimeSpan.FromSeconds(30);
			})
			.AddHttpMessageHandler<AuthHttpMessageHandler>();
		
		builder.Services.AddSingleton<AuthHttpMessageHandler>();

#if DEBUG
		builder.Logging.AddDebug();
#endif
		
#if ANDROID
    EntryHandler.Mapper.AppendToMapping(nameof(BorderlessEntry), (handler, view) =>
    {
        if (view is NYC.MobileApp.Controls.BorderlessEntry)
        {
            handler.PlatformView.Background = null;
			handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            //handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
            //handler.PlatformView.SetPadding(0, 0, 0, 0);
        }
    });
#endif
		builder.Services.AddViewModel<ProductDetailsViewModel, ProductDetailsView>();

		return builder.Build();
	}
	
	private static void AddViewModel<TViewModel, TView>(this IServiceCollection services)
		where TView : ContentPage, new()
		where TViewModel : class
	{
		services.AddTransient<TViewModel>();
		services.AddTransient<TView>(s => new TView() { BindingContext = s.GetRequiredService<TViewModel>() });
	}
}
