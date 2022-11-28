using CommunityToolkit.Maui;
using Pileolinks.Services;
using Pileolinks.Services.Interfaces;
using Pileolinks.ViewModels;
using Pileolinks.ViewModels.Factories;
using Pileolinks.ViewModels.Factories.Interfaces;
using Pileolinks.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Pileolinks;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("FluentSystemIcons-Regular.ttf", "Fluent");
			});

		builder.Services.AddSingleton<IDataConverter, DataConverter>();
		builder.Services.AddSingleton<IDataService, LocalStorageDataService>();
		builder.Services.AddTransient<MainPageViewModel>();
		builder.Services.AddTransient<SearchViewModel>();
		builder.Services.AddTransient<TreeFlyout>();
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<Search>();
		builder.Services.AddTransient<DirectoryView>();
		builder.Services.AddSingleton<ILinkViewModelFactory, LinkViewModelFactory>();
		builder.Services.AddSingleton<ILinkDirectoryViewModelFactory, DirectoryViewModelFactory>();

		return builder.Build();
	}
}
