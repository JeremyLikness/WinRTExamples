using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers.Provider;
using Windows.Storage.Search;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using IntegrationExampleLauncher.Common;

namespace IntegrationExampleLauncher
{
    /// <summary>
    /// This page displays files owned by the application so that the user can grant another application
    /// access to them.
    /// </summary>
    public sealed partial class InternalFileListingPickerPage : Page
    {
        /// <summary>
        /// Files are added to or removed from the Windows UI to let Windows know what has been selected.
        /// </summary>
        private FileOpenPickerUI _fileOpenPickerUI;
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

        public InternalFileListingPickerPage()
        {
            InitializeComponent();
            Window.Current.SizeChanged += Window_SizeChanged;
            InvalidateVisualState();
        }

        void Window_SizeChanged(Object sender, WindowSizeChangedEventArgs e)
        {
            InvalidateVisualState();
        }

        private void InvalidateVisualState()
        {
            var visualState = DetermineVisualState();
            VisualStateManager.GoToState(this, visualState, false);
        }

        private string DetermineVisualState()
        {
            return Window.Current.Bounds.Width >= 500 ? "HorizontalView" : "VerticalView";
        }

        /// <summary>
        /// Invoked when another application wants to open files from this application.
        /// </summary>
        /// <param name="e">Activation data used to coordinate the process with Windows.</param>
        public async void Activate(FileOpenPickerActivatedEventArgs e)
        {
            _fileOpenPickerUI = e.FileOpenPickerUI;
            _fileOpenPickerUI.FileRemoved += FilePickerUI_FileRemoved;

            DefaultViewModel["CanGoUp"] = false;
            Window.Current.Content = this;
            Window.Current.Activate();

            // TODO: Set this.DefaultViewModel["Files"] to show a collection of items,
            //       each of which should have bindable Image, Title, and Description

            // Now that the window has been activated, go ahead and load up the files
            var appInstalledLocation = Package.Current.InstalledLocation;
            var filesFolder = await appInstalledLocation.GetFolderAsync("FileActivation");
            var queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, new[] {".wrtbe"});
            var query = filesFolder.CreateFileQueryWithOptions(queryOptions);
            var files = await query.GetFilesAsync();

            var filesList = new List<Object>();
            foreach (var file in files)
            {
                var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.ListView);
                if (thumbnail != null)
                {
                    var image = new BitmapImage();
                    image.SetSource(thumbnail);
                    var result = new TrackedFile
                    {
                        Id = file.FolderRelativeId,
                        Title = file.Name,
                        Image = image,
                        StorageFile = file,
                    };
                    filesList.Add(result);
                }
            }

            DefaultViewModel["Files"] = filesList;
        }

        /// <summary>
        /// Invoked when user removes one of the items from the Picker basket
        /// </summary>
        /// <param name="sender">The FileOpenPickerUI instance used to contain the available files.</param>
        /// <param name="e">Event data that describes the file removed.</param>
        private void FilePickerUI_FileRemoved(FileOpenPickerUI sender, FileRemovedEventArgs e)
        {
            throw new InvalidOperationException("Only single selection is currently supported.");

            // TODO: Respond to an item being deselected in the picker UI.
        }

        /// <summary>
        /// Invoked when the selected collection of files changes.
        /// </summary>
        /// <param name="sender">The GridView instance used to display the available files.</param>
        /// <param name="e">Event data that describes how the selection changed.</param>
        private void FileGridView_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            foreach (TrackedFile removedItem in e.RemovedItems)
            {
                _fileOpenPickerUI.RemoveFile(removedItem.Id);
            }
            foreach (TrackedFile addedItem in e.AddedItems)
            {
                _fileOpenPickerUI.AddFile(addedItem.Id, addedItem.StorageFile);
            }
        }

        /// <summary>
        /// Invoked when the "Go up" button is clicked, indicating that the user wants to pop up
        /// a level in the hierarchy of files.
        /// </summary>
        /// <param name="sender">The Button instance used to represent the "Go up" command.</param>
        /// <param name="e">Event data that describes how the button was clicked.</param>
        private void GoUpButton_Click(Object sender, RoutedEventArgs e)
        {
            // TODO: Set this.DefaultViewModel["CanGoUp"] to true to enable the corresponding command,
            //       use updates to this.DefaultViewModel["Files"] to reflect file hierarchy traversal
        }

        private class TrackedFile
        {
            public String Id { get; set; }
            public String Title { get; set; }
            public String Description { get; set; }
            public BitmapImage Image { get; set; }
            public IStorageFile StorageFile { get; set; }
        }
    }
}
