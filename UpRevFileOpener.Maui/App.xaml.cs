namespace UpRevFileOpener;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Check if product key has been entered
        bool productKeyEntered = Preferences.Get("ProductKeyEntered", false);

        if (productKeyEntered)
        {
            MainPage = new AppShell();
        }
        else
        {
            MainPage = new NavigationPage(new LoginPage());
        }
    }
}
