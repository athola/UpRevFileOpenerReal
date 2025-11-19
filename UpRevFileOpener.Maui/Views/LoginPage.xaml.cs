using UpRevFileOpener.Services;

namespace UpRevFileOpener;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        productKey.Focus();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // Validate that the product key is all digits
        if (!productKey.Text?.All(char.IsDigit) ?? true)
        {
            await DisplayAlert("Invalid Input", "This is a number only field", "OK");
            productKey.Text = "";
            return;
        }

        // Validate length
        if (productKey.Text?.Length < 16)
        {
            await DisplayAlert("Invalid Input", "You entered less than 16 digits", "OK");
            productKey.Text = "";
            return;
        }

        // Save the product key as entered
        SettingsService.ProductKeyEntered = true;

        // Navigate to the main application
        Application.Current!.MainPage = new AppShell();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        // Close the application
        Application.Current?.Quit();
    }
}
