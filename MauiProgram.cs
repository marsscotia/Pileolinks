using CommunityToolkit.Maui;
using Pileolinks.Services;
using Pileolinks.Services.Interfaces;

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

		return builder.Build();
	}
}
