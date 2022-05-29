namespace Pileolinks;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        MainPage = new MainPage();
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
}
