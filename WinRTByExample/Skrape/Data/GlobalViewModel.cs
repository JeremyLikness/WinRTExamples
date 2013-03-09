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
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Skrape.Common;

    using Windows.ApplicationModel.DataTransfer;
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
                () => this.DeleteButtonEnabled);

            this.RefreshCommand = new ActionCommand(
                async () => await this.RefreshCallback(), 
                () => this.DeleteButtonEnabled);

            this.FavoriteCommand = new ActionCommand(
                async () => await this.MakeFavorite(),
                () => this.FavoriteButtonEnabled);

            this.CopyCommand = new ActionCommand(
                async () => await this.Copy(),
                () => this.DeleteButtonEnabled);

            this.PasteCommand = new ActionCommand(
                async () => await this.Paste(), 
                () => this.CanPaste);
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
                        ((ActionCommand)this.PasteCommand).RaiseExecuteChanged();
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
        private bool DeleteButtonEnabled
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
        /// Gets a value indicating whether can paste.
        /// </summary>
        private bool CanPaste
        {
            get
            {
                return this.DataManager.CurrentPasteUri != null;
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
            Clipboard.SetContent(package);
            var dialog = new MessageDialog("The text for the web page was successfully copied to the clipboard.");
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
            await this.DataManager.AddUrl(this.DataManager.CurrentPasteUri);
            DataManager.CurrentPasteUri = null;
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
            if (!this.DeleteButtonEnabled)
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
