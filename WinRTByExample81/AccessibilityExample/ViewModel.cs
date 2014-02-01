namespace AccessibilityExample
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Windows.Storage;
    using Windows.UI.Xaml;

    public class ViewModel : INotifyPropertyChanged
    {
        private ElementTheme currentTheme = Application.Current.RequestedTheme == ApplicationTheme.Dark ? ElementTheme.Dark : ElementTheme.Light;

        private readonly IEnumerable<Item> items = new List<Item>
                                              {
                                                  new Item { Id = 1, Description = "List Item 1" },
                                                  new Item { Id = 2, Description = "List Item 2" },
                                                  new Item { Id = 3, Description = "List Item 3" }
                                              };

        private Item currentItem, currentItem2;

        public ViewModel()
        {
            this.currentItem = this.currentItem2 = this.items.First();
        }

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

        public Item CurrentItem
        {
            get
            {
                return currentItem;
            }

            set
            {
                currentItem = value;
                this.OnPropertyChanged();
            }
        }

        public Item CurrentItem2
        {
            get
            {
                return currentItem2;
            }

            set
            {
                currentItem2 = value;
                this.OnPropertyChanged();
            }
        }



        public IEnumerable<Item> Items
        {
            get
            {
                return this.items;
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