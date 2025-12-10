using System.Windows;
using Tomany.TaskManagement.BLL.Models;
using Tomany.TaskManagement.BLL.Services;

namespace Tomany.TaskManagement
{
    public partial class RegisterWindow : Window
    {
        private readonly AccountService _accountService;
        private readonly OtpService _otpService;
        private bool _isSubmitting;
        private bool _emailVerified;

        public RegisterWindow()
        {
            InitializeComponent();
            var dbContext = new Tomany.TaskManagement.DAL.Models.TaskManagementContext();
            var accountRepo = new Tomany.TaskManagement.DAL.Repositories.AccountRepository(dbContext);
            _accountService = new AccountService(accountRepo);
            
            // Configure email service (update these with your SMTP credentials)
            var smtpSettings = GetSmtpSettings();
            var emailService = smtpSettings != null ? new EmailService(smtpSettings) : null;
            _otpService = new OtpService(emailService);
        }

        private SmtpSettings? GetSmtpSettings()
        {
            // TODO: Configure your SMTP settings here
            // For Gmail, you need to:
            // 1. Enable "Less secure app access" or use App Password
            // 2. Replace with your Gmail credentials
            
            // Example for Gmail:
            // return new SmtpSettings(
            //     username: "your-email@gmail.com",
            //     password: "your-app-password",
            //     fromEmail: "your-email@gmail.com"
            // );
            
            // Return null to disable email sending (will show OTP in MessageBox)
            return new SmtpSettings(
                username: "vientruongdoan@gmail.com",
                password: "grku ryyd ldsr gjov",
                fromEmail: "vientruongdoan@gmail.com"
            );
        }

        private async void SendOtpButton_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Please enter email before sending OTP.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SendOtpButton.IsEnabled = false;
            UpdateStatus("Sending OTP...");

            try
            {
                var code = await _otpService.GenerateAndSendOtpAsync(email);
                _emailVerified = false;
                UpdateStatus("OTP sent. Please check your email.");
                MessageBox.Show("OTP has been sent to your email address.", "OTP Sent", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Failed to send OTP: {ex.Message}");
                MessageBox.Show($"Failed to send OTP email: {ex.Message}\n\nPlease check your SMTP configuration.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SendOtpButton.IsEnabled = true;
            }
        }

        private void VerifyOtpButton_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailTextBox.Text?.Trim();
            var code = OtpTextBox.Text?.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("Email and OTP are required to verify.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var ok = _otpService.ValidateOtp(email, code);
            if (ok)
            {
                _emailVerified = true;
                UpdateStatus("Email verified. You can now create the account.");
                MessageBox.Show("Email verified successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                _emailVerified = false;
                UpdateStatus("OTP is invalid or expired.");
                MessageBox.Show("Invalid or expired OTP.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isSubmitting)
            {
                return;
            }

            if (!_emailVerified)
            {
                MessageBox.Show("Please verify email via OTP before creating the account.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _isSubmitting = true;
            RegisterButton.IsEnabled = false;
            UpdateStatus("Submitting...");

            var request = new RegisterRequest
            {
                Username = UsernameTextBox.Text,
                Password = PasswordBox.Password,
                ConfirmPassword = ConfirmPasswordBox.Password,
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Email = EmailTextBox.Text,
                PhoneNumber = PhoneTextBox.Text,
                DateOfBirth = DobPicker.SelectedDate
            };

            var result = await _accountService.RegisterAsync(request);

            if (result.Success)
            {
                MessageBox.Show(result.Message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateStatus($"Created account (ID: {result.AccountId}).");
                ClearForm();
            }
            else
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                UpdateStatus(result.Message);
            }

            RegisterButton.IsEnabled = true;
            _isSubmitting = false;
        }

        private void BackToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            UpdateStatus("Cleared.");
        }

        private void ClearForm()
        {
            UsernameTextBox.Text = string.Empty;
            EmailTextBox.Text = string.Empty;
            PasswordBox.Password = string.Empty;
            ConfirmPasswordBox.Password = string.Empty;
            FirstNameTextBox.Text = string.Empty;
            LastNameTextBox.Text = string.Empty;
            PhoneTextBox.Text = string.Empty;
            DobPicker.SelectedDate = null;
            OtpTextBox.Text = string.Empty;
            _emailVerified = false;
        }

        private void UpdateStatus(string message)
        {
            StatusTextBlock.Text = message;
        }
    }
}

