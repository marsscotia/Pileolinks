namespace Pileolinks;

public partial class App : Application
{
	/// <summary>
    /// Note to future self: We don't seem to be able to 
    /// inject the MainPage type into the constructor here,
    /// so we inject the service provider and resolve the type
    /// manually.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();

		MainPage = new MainPage();
	}
}
