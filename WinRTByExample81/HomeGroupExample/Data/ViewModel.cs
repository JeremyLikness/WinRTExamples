namespace HomeGroupExample.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Windows.Storage.FileProperties;
    using Windows.Storage.Search;
    using Windows.UI.Xaml.Media.Imaging;

    using HomeGroupExample.Common;

    public class ViewModel : INotifyPropertyChanged
    {
        private HomeGroupUser selectedUser;

        public ViewModel()
        {
            Users = new ObservableCollection<HomeGroupUser>();
            Images = new ObservableCollection<ImageItem>();
            PickUser = new ActionCommand(async obj => await SelectForUser(obj), IsValidUser);

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

        private async Task SelectForUser(object user)
        {
            var homeGroupUser = user as HomeGroupUser;
            if (homeGroupUser != null)
            {
                var targetFolder =
                    (await Windows.Storage.KnownFolders.HomeGroup.GetFoldersAsync()).FirstOrDefault(
                        folder => folder.DisplayName == homeGroupUser.UserName);
                if (targetFolder != null)
                {
                    var query = new QueryOptions(CommonFileQuery.OrderBySearchRank, new[] { ".jpg", ".png", ".bmp", ".gif" })
                                    {
                                        UserSearchFilter =
                                            "kind:picture"
                                    };
                    var files = await targetFolder.CreateFileQueryWithOptions(query).GetFilesAsync();
                    foreach (var file in files)
                    {
                        var image = new ImageItem { Name = file.DisplayName };
                        var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.PicturesView, 200);
                        image.Image = new BitmapImage();
                        image.Image.SetSource(thumbnail);
                        Images.Add(image);
                    }
                }
            }
        }

        private bool IsValidUser(object user)
        {
            var homeGroupUser = user as HomeGroupUser;
            return homeGroupUser != null && homeGroupUser.IsHomeGroupUser;
        }

        private void PopulateDesignData()
        {
            var user1 = new HomeGroupUser { UserName = "John Garland", IsHomeGroupUser = true };
            var user2 = new HomeGroupUser { UserName = "Jeremy Likness", IsHomeGroupUser = true };
            Users.Add(user1);
            Users.Add(user2);
            SelectedUser = user1;
            var image = new ImageItem
                            {
                                Name = "Sample Image",
                                Image = new BitmapImage(new Uri("ms-appx:///Assets/Logo.scale-100.png"))
                            };
            Images.Add(image);
        }

        public ObservableCollection<HomeGroupUser> Users { get; set; }

        public ObservableCollection<ImageItem> Images { get; set; } 

        public ActionCommand PickUser { get; private set; }

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