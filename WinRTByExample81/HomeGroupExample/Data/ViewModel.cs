namespace HomeGroupExample.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public class ViewModel : INotifyPropertyChanged
    {
        private HomeGroupUser selectedUser;

        public ViewModel()
        {
            Users = new ObservableCollection<HomeGroupUser>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                PopulateDesignData();
            }
        }

        public async Task Initialize()
        {
            try
            {
                var folders = await Windows.Storage.KnownFolders.HomeGroup.GetFoldersAsync();
                foreach (
                    var user in
                        folders.Select(
                            folder => new HomeGroupUser { UserName = folder.DisplayName, IsHomeGroupUser = true }))
                {
                    this.Users.Add(user);
                }
            }
            catch (Exception ex)
            {
                var user = new HomeGroupUser
                               {
                                   UserName = string.Format("Error: {0}", ex.Message),
                                   IsHomeGroupUser = false
                               };
                Users.Add(user);
            }
        }

        private void PopulateDesignData()
        {
            var user1 = new HomeGroupUser { UserName = "John Garland", IsHomeGroupUser = true };
            var user2 = new HomeGroupUser { UserName = "Jeremy Likness", IsHomeGroupUser = true };
            Users.Add(user1);
            Users.Add(user2);
            SelectedUser = user1; 
        }

        public ObservableCollection<HomeGroupUser> Users { get; set; }

        public HomeGroupUser SelectedUser
        {
            get
            {
                return this.selectedUser;
            }
            set
            {
                this.selectedUser = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}