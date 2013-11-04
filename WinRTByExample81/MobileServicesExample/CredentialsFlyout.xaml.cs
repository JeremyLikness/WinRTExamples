using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace MobileServicesExample
{
    public sealed partial class CredentialsFlyout : SettingsFlyout
    {
        private readonly CredentialsViewModel _viewModel;
        public CredentialsFlyout()
        {
            _viewModel = new CredentialsViewModel();
            _viewModel.LoginCommand.CanExecuteChanged += (sender, args) => UpdateButtonVisibility();
            _viewModel.LogoutCommand.CanExecuteChanged += (sender, args) => UpdateButtonVisibility();
            
            InitializeComponent();

            UpdateButtonVisibility();
        }

        public CredentialsViewModel ViewModel
        {
            get { return _viewModel; }
        }

        private void UpdateButtonVisibility()
        {
            LoginButton.Visibility = _viewModel.LoginCommand.CanExecute(null) ? Visibility.Visible : Visibility.Collapsed;
            LogoutButton.Visibility = _viewModel.LogoutCommand.CanExecute(null) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
