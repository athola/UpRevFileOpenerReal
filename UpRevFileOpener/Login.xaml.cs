using System.Linq;
using System.Windows;

namespace UpRevFileOpener
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!productKey.Text.All(char.IsDigit))
            {
                MessageBox.Show("This is a number only field");
                productKey.Text = "";
                return;
            }
            else if (productKey.Text.Length < 16)
            {
                MessageBox.Show("You entered less than 16 digits");
                productKey.Text = "";
                return;
            }
            Properties.Settings.Default.ProductKeyEntered = true;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
