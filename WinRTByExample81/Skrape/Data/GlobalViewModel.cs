// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalViewModel.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The global resource.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Skrape.Data
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Common;

    using Windows.ApplicationModel.DataTransfer;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI.Popups;

    /// <summary>
    /// The global resource.
    /// </summary>
    /// <remarks>
    /// This is placed in the App resources and then used to configure properties to reference globally through XAML
    /// </remarks>
    public class GlobalViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The data manager.
        /// </summary>
        private SkrapeDataManager dataManager;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalViewModel"/> class.
        /// </summary>
        public GlobalViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                DataManager = new SkrapeDataManager();
            }

            GoHomeCommand = new ActionCommand(
                () => GoHome(),
                () => HomeButtonEnabled);

            DeleteCommand = new ActionCommand(
                async () => await Delete(),
                () => DetailPageEnabled);

            RefreshCommand = new ActionCommand(
                async () => await RefreshCallback(), 
                () => DetailPageEnabled);

            FavoriteCommand = new ActionCommand(
                async () => await MakeFavorite(),
                () => FavoriteButtonEnabled);

            CopyCommand = new ActionCommand(
                async () => await Copy(),
                () => DetailPageEnabled);

            SaveCommand = new ActionCommand(
                async () => await Save(), 
                () => FavoriteButtonEnabled);

            PasteCommand = new ActionCommand(
                async () => await Paste(), 
                () => true);

            DownloadCommand = new ActionCommand(
                async () => await Download(),
                () => DetailPageEnabled);

            AddCommand = new ActionCommand(
                () => AddCallback(),
                () => !DetailPageEnabled);
        }        
        
        /// <summary>
        /// The property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Gets or sets the go home.
        /// </summary>
        public Action GoHome { get; set; }

        /// <summary>
        /// Gets or sets the delegate to refresh the page
        /// </summary>
        public Func<Task> RefreshCallback { get; set; }

        /// <summary>
        /// Gets or sets the add callback.
        /// </summary>
        public Action AddCallback { get; set; }

        /// <summary>
        /// Gets the command to go home
        /// </summary>
        public ICommand GoHomeCommand { get; private set; }

        /// <summary>
        /// Gets the command to add a new page
        /// </summary>
        public ICommand AddCommand { get; private set; }

        /// <summary>
        /// Gets the command to copy to the clipboard
        /// </summary>
        public ICommand CopyCommand { get; private set; }

        /// <summary>
        /// Gets the command to download the text
        /// </summary>
        public ICommand DownloadCommand { get; private set; }

        /// <summary>
        /// Gets the command to set an image as the favorite
        /// </summary>
        public ICommand FavoriteCommand { get; private set; }

        /// <summary>
        /// Gets the command to save an image to the file system
        /// </summary>
        public ICommand SaveCommand { get; private set; }

        /// <summary>
        /// Gets the command to paste a URL 
        /// </summary>
        public ICommand PasteCommand { get; private set; }

        /// <summary>
        /// Gets the command to refresh the page information
        /// </summary>
        public ICommand RefreshCommand { get; private set; }

        /// <summary>
        /// Gets the command to delete an item
        /// </summary>
        public ICommand DeleteCommand { get; private set; }

        /// <summary>
        /// Gets or sets the data manager.
        /// </summary>
        public SkrapeDataManager DataManager
        {
            get
            {
                return dataManager;
            }

            set
            {
                dataManager = value;
// ReSharper disable ExplicitCallerInfoArgument
                dataManager.PropertyChanged += (o, e) =>
                    { 
                        OnPropertyChanged(string.Empty);
                        ((ActionCommand)GoHomeCommand).RaiseExecuteChanged();
                        ((ActionCommand)DeleteCommand).RaiseExecuteChanged();
                        ((ActionCommand)FavoriteCommand).RaiseExecuteChanged();
                        ((ActionCommand)SaveCommand).RaiseExecuteChanged();
                        ((ActionCommand)RefreshCommand).RaiseExecuteChanged();
                        ((ActionCommand)CopyCommand).RaiseExecuteChanged();   
                        ((ActionCommand)AddCommand).RaiseExecuteChanged();
                        ((ActionCommand)DownloadCommand).RaiseExecuteChanged();
                    };

// ReSharper restore ExplicitCallerInfoArgument
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether home button enabled.
        /// </summary>
        private bool HomeButtonEnabled
        {
            get
            {
                return dataManager.CurrentPage != null && dataManager.CurrentPage.Id > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the delete button should be enabled
        /// </summary>
        private bool DetailPageEnabled
        {
            get
            {
                return dataManager.CurrentPage != null && dataManager.CurrentPage.Id > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the favorite button should be enabled
        /// </summary>
        private bool FavoriteButtonEnabled
        {
            get
            {
                return dataManager.CurrentImage != null;
            }
        }

        /// <summary>
        /// The on property changed event.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// The copy.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> to run asynchronously.
        /// </returns>
        private async Task Copy()
        {
            var package = new DataPackage();
            package.SetText(DataManager.CurrentPage.Text);            
            package.SetHtmlFormat(HtmlFormatHelper.CreateHtmlFormat(DataManager.CurrentPage.Html));
            Clipboard.SetContent(package);
            var dialog = new MessageDialog("The web page was successfully copied to the clipboard.");
            await dialog.ShowAsync();
        }

        /// <summary>
        /// The download.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> to run asynchronously.
        /// </returns>
        private async Task Download()
        {
            var page = DataManager.CurrentPage;
            var url = page.Url.ToString();
            var nameOnDisk =
                url.Substring(url.LastIndexOf("//", StringComparison.CurrentCultureIgnoreCase) + 2)
                    .Replace(".", "_")
                    .Replace("/", "-");
            if (nameOnDisk.EndsWith("-"))
            {
                nameOnDisk = nameOnDisk.Substring(0, nameOnDisk.Length - 1);
            }

            var filename = string.Format("{0}.txt", nameOnDisk);
            var download = await DownloadsFolder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);

            // await FileIO.WriteTextAsync(download, page.Text, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            using (IRandomAccessStream stream = await download.OpenAsync(FileAccessMode.ReadWrite))
            {
                await stream.WriteAsync(Encoding.UTF8.GetBytes(page.Text).AsBuffer());
                await stream.FlushAsync();
            }

            var dialog = new MessageDialog(
                "Successfully downloaded the page as text to your Downloads folder.", 
                download.Name);
            await dialog.ShowAsync();
        }

        /// <summary>
        /// The paste.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> asynchronous task.
        /// </returns>
        private async Task Paste()
        {
            MessageDialog dialog;
            try
            {
                var clipboardData = Clipboard.GetContent();

                var urlAdded = false;
                var uri = new Uri("http://csharperimage.jeremylikness.com/", UriKind.Absolute); // default initial value

                if (clipboardData.Contains(StandardDataFormats.WebLink))
                {
                    uri = await clipboardData.GetWebLinkAsync();
                    urlAdded = true;
                }
                else if (clipboardData.Contains(StandardDataFormats.Text))
                {
                    var text = await clipboardData.GetTextAsync();
                    if (Uri.TryCreate(text, UriKind.Absolute, out uri))
                    {
                        urlAdded = true;
                    }
                }

                if (urlAdded)
                {
                    await DataManager.AddUrl(uri);
                    dialog = new MessageDialog("The URL was added.");
                }
                else
                {
                    dialog = new MessageDialog("There is no URL in the clipboard.");
                }                                
            }
            catch (Exception ex)
            {
                dialog = new MessageDialog(ex.Message, "An Error Occurred");
            }

            await dialog.ShowAsync();
        }

        /// <summary>
        /// The make favorite.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> to complete asynchronously.
        /// </returns>
        private async Task MakeFavorite()
        {
            var uri =
                await dataManager.ProcessImage(
                dataManager.CurrentImage,
                dataManager.CurrentPage.Id);
            dataManager.CurrentPage.ThumbnailPath = uri;
            await DataManager.Manager.SavePage(dataManager.CurrentPage);
        }

        /// <summary>
        /// The save method.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> to complete asynchronous.
        /// </returns>
        private async Task Save()
        {
            await dataManager.SaveImage(dataManager.CurrentImage);
        }

        /// <summary>
        /// The delete task
        /// </summary>
        /// <returns>The asynchronous task</returns>
        private async Task Delete()
        {
            if (!DetailPageEnabled)
            {
                return;
            }

            var dialog = new MessageDialog("Are you sure you wish to delete this skrape?", "Confirm Delete");
            dialog.Commands.Add(
                new UICommand(
                    "Delete",
                    c =>
                    {
                        DataManager.DeletePage(DataManager.CurrentPage);
                        GoHome();
                    }));
            dialog.Commands.Add(new UICommand("Cancel", c => { }));
            dialog.CancelCommandIndex = 1;
            dialog.DefaultCommandIndex = 1;
            await dialog.ShowAsync();
        }
    }
}
