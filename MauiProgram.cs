using CommunityToolkit.Maui;

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

		return builder.Build();
	}
}
