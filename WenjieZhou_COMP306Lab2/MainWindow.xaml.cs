// Author: Wenjie Zhou
// Student ID: 301337168
// Course: COMP 306
// Date: October 1, 2024


using System.Windows;


namespace WenjieZhou_COMP306Lab2
{
    public partial class MainWindow : Window
    {
        private readonly MainOperation _mainOperation;

        public MainWindow()
        {
            InitializeComponent();

            _mainOperation = new MainOperation();
        }

        // Login button click event handler
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = emailTextBox.Text;
            string password = passwordBox.Password;

            // Check if the email and password are empty
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Email or password cannot be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Check if the email and password are valid
                bool isAuthenticated = await _mainOperation.Login(email, password);

                if (isAuthenticated)
                {
                    var bookshelfWindow = new BookshelfWindow(email);

                    bookshelfWindow.Show();
                    this.Hide();

                    // Clear the fields after successful login
                    emailTextBox.Clear();
                    passwordBox.Clear();
                }
                else
                {
                    MessageBox.Show("Invalid email or password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Exit button click event handler
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Dispose the Amazon DynamoDB and S3 clients
            Helper.amazonDynamoDBClient?.Dispose();
            Helper.s3Client?.Dispose();

            // Close the application
            Application.Current.Shutdown();
        }
    }
}