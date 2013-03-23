﻿// --------------------------------------------------------------------------------------------------------------------
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

    using Skrape.Common;

    using Windows.ApplicationModel.DataTransfer;
    using Windows.Storage;
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
                this.DataManager = new SkrapeDataManager();
            }

            this.GoHomeCommand = new ActionCommand(
                () => this.GoHome(),
                () => this.HomeButtonEnabled);

            this.DeleteCommand = new ActionCommand(
                async () => await this.Delete(),
                () => this.DetailPageEnabled);

            this.RefreshCommand = new ActionCommand(
                async () => await this.RefreshCallback(), 
                () => this.DetailPageEnabled);

            this.FavoriteCommand = new ActionCommand(
                async () => await this.MakeFavorite(),
                () => this.FavoriteButtonEnabled);

            this.CopyCommand = new ActionCommand(
                async () => await this.Copy(),
                () => this.DetailPageEnabled);

            this.PasteCommand = new ActionCommand(
                async () => await this.Paste(), 
                () => true);

            this.DownloadCommand = new ActionCommand(
                async () => await this.Download(),
                () => this.DetailPageEnabled);

            this.AddCommand = new ActionCommand(
                () => this.AddCallback(),
                () => !this.DetailPageEnabled);
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
                return this.dataManager;
            }

            set
            {
                this.dataManager = value;
// ReSharper disable ExplicitCallerInfoArgument
                this.dataManager.PropertyChanged += (o, e) =>
                    { 
                        this.OnPropertyChanged(string.Empty);
                        ((ActionCommand)this.GoHomeCommand).RaiseExecuteChanged();
                        ((ActionCommand)this.DeleteCommand).RaiseExecuteChanged();
                        ((ActionCommand)this.FavoriteCommand).RaiseExecuteChanged();
                        ((ActionCommand)this.RefreshCommand).RaiseExecuteChanged();
                        ((ActionCommand)this.CopyCommand).RaiseExecuteChanged();   
                        ((ActionCommand)this.AddCommand).RaiseExecuteChanged();
                        ((ActionCommand)this.DownloadCommand).RaiseExecuteChanged();
                    };

// ReSharper restore ExplicitCallerInfoArgument
                this.OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether home button enabled.
        /// </summary>
        private bool HomeButtonEnabled
        {
            get
            {
                return this.dataManager.CurrentPage != null && this.dataManager.CurrentPage.Id > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the delete button should be enabled
        /// </summary>
        private bool DetailPageEnabled
        {
            get
            {
                return this.dataManager.CurrentPage != null && this.dataManager.CurrentPage.Id > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the favorite button should be enabled
        /// </summary>
        private bool FavoriteButtonEnabled
        {
            get
            {
                return this.dataManager.CurrentImage != null;
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
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
            package.SetText(this.DataManager.CurrentPage.Text);            
            package.SetHtmlFormat(HtmlFormatHelper.CreateHtmlFormat(this.DataManager.CurrentPage.Html));
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
            var page = this.DataManager.CurrentPage;
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
            using (var stream = await download.OpenAsync(FileAccessMode.ReadWrite))
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

                if (!clipboardData.Contains(StandardDataFormats.Uri))
                {
                    dialog = new MessageDialog("There is no URL in the clipboard.");
                }
                else
                {
                    var uri = await clipboardData.GetUriAsync();
                    await this.DataManager.AddUrl(uri);
                    dialog = new MessageDialog("The URL was added.");
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
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task MakeFavorite()
        {
            var uri =
                await this.dataManager.ProcessImage(
                this.dataManager.CurrentImage,
                this.dataManager.CurrentPage.Id);
            this.dataManager.CurrentPage.ThumbnailPath = uri;
            await this.DataManager.Manager.SavePage(this.dataManager.CurrentPage);
        }

        /// <summary>
        /// The delete task
        /// </summary>
        /// <returns>The asynchronous task</returns>
        private async Task Delete()
        {
            if (!this.DetailPageEnabled)
            {
                return;
            }

            var dialog = new MessageDialog("Are you sure you wish to delete this skrape?", "Confirm Delete");
            dialog.Commands.Add(
                new UICommand(
                    "Delete",
                    c =>
                    {
                        this.DataManager.DeletePage(this.DataManager.CurrentPage);
                        this.GoHome();
                    }));
            dialog.Commands.Add(new UICommand("Cancel", c => { }));
            dialog.CancelCommandIndex = 1;
            dialog.DefaultCommandIndex = 1;
            await dialog.ShowAsync();
        }
    }
}