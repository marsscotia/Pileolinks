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
        
        MainPage = (MainPage)serviceProvider.GetRequiredService(typeof(MainPage));
	}

    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = base.CreateWindow(activationState);

        window.Created += (s, e) =>
        {
            (s as Window).Stopped += App_Stopped;
        };

		return window;
    }

    private void App_Stopped(object sender, EventArgs e)
    {
        (MainPage as MainPage).TreeFlyout.ViewModel.SaveState();
    }

    protected override void OnSleep()
    {
        (MainPage as MainPage).TreeFlyout.ViewModel.SaveState();
        base.OnSleep();
    }
}
