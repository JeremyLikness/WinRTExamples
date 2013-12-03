namespace PortableMVVM
{
    using System.ComponentModel;
    using System.Windows.Input;

    public class PortableViewModel : INotifyPropertyChanged
    {
        private string tapText;

        public PortableViewModel()
        {
            this.TapCommand = new RunOnceCommand(this.OnTapped);
            this.TapText = "Tap or Click Me.";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand TapCommand { get; private set; }

        public string TapText
        {
            get
            {
                return this.tapText;
            }

            set
            {
                if (value == this.tapText)
                {
                    return;
                }

                this.tapText = value;
                this.OnPropertyChanged("TapText");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnTapped()
        {
            this.TapText = "Disabled.";
        }
    }
}
