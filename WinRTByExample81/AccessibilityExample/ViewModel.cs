namespace AccessibilityExample
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Windows.Storage;
    using Windows.UI.Xaml;

    public class ViewModel : INotifyPropertyChanged
    {
        private ElementTheme currentTheme = Application.Current.RequestedTheme == ApplicationTheme.Dark ? ElementTheme.Dark : ElementTheme.Light;

        public ElementTheme CurrentTheme
        {
            get
            {
                return currentTheme;
            }

            set
            {
                currentTheme = value; 
                this.OnPropertyChanged();
            }
        }

        public void ToggleTheme()
        {
            this.CurrentTheme = this.currentTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
            ApplicationData.Current.RoamingSettings.Values["Theme"] = this.CurrentTheme == ElementTheme.Light ? "Light" : "Dark";
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));           
        }
    }
}