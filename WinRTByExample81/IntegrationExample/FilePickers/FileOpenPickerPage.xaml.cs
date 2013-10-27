using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Contacts;
using Windows.Storage.Pickers.Provider;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using IntegrationExample.Common;
using IntegrationExample.Data;

namespace IntegrationExample
{
    /// <summary>
    /// This page displays files owned by the application so that the user can grant another application
    /// access to them.
    /// </summary>
    public sealed partial class FileOpenPickerPage : Page
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

        /// <summary>
        /// Initializes a new instance of the <see cref="FileOpenPickerPage"/> class.
        /// </summary>
        public FileOpenPickerPage()
        {
            InitializeComponent();
            Window.Current.SizeChanged += HandleWindowSizeChanged;
            InvalidateVisualState();
        }

        private void HandleWindowSizeChanged(Object sender, WindowSizeChangedEventArgs e)
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
        /// Used to set up the content to be displayed and connect to the containing File Open Picker
        /// </summary>
        /// <param name="fileOpenPickerUI">The file open picker UI element.</param>
        public void Initialize(FileOpenPickerUI fileOpenPickerUI)
        {
            if (fileOpenPickerUI == null) throw new ArgumentNullException("fileOpenPickerUI");

            // Tie into the containing File Picker object
            _fileOpenPickerUI = fileOpenPickerUI;
            _fileOpenPickerUI.FileRemoved += HandleFilePickerUIFileRemoved;

            // Initialize the ViewModel
            DefaultViewModel["CanGoUp"] = false;
            DefaultViewModel["SelectionMode"] =
                _fileOpenPickerUI.SelectionMode == FileSelectionMode.Multiple
                    ? ListViewSelectionMode.Multiple
                    : ListViewSelectionMode.Single;
            DefaultViewModel["Contacts"] = null;
            DefaultViewModel["SelectedContact"] = null;

            // Load the contact info 
            LoadContacts();
        }

        private void LoadContacts()
        {
            // Fetch the data and set it to the View Model
            var sampleDataGroups = Application.Current.GetSampleData().Groups;
            var allContacts =
                sampleDataGroups.SelectMany(group => group.Items)
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .ToList();
            DefaultViewModel["Contacts"] = allContacts;
            DefaultViewModel["SelectedContact"] = allContacts.FirstOrDefault();
        }

        private async void HandleSelectedContactChanged(Object sender, SelectionChangedEventArgs e)
        {
            // Determine the selected contact (if there is one) and retrieve the list of associated files.
            var files = new List<FileInfo>();
            if (DefaultViewModel["SelectedContact"] != null)
            {
                var currentContact = (Contact)DefaultViewModel["SelectedContact"];
                files = await currentContact.GetRelatedFiles();
            }
            DefaultViewModel["Files"] = files;
        }

        /// <summary>
        /// Invoked when the selected collection of files changes.
        /// </summary>
        /// <param name="sender">The GridView instance used to display the available files.</param>
        /// <param name="e">Event data that describes how the selection changed.</param>
        private void HandleSelectedFileChanged(Object sender, 
            SelectionChangedEventArgs e)
        {
            // Update the picker 'basket' with the newly selected items
            foreach (var addedFile in e.AddedItems.Cast<FileInfo>())
            {
                if (_fileOpenPickerUI.CanAddFile(addedFile.File))
                {
                    _fileOpenPickerUI.AddFile(addedFile.Title, addedFile.File);
                }
            }

            // Update the picker 'basket' to remove any newly removed items
            foreach (var removedFile in e.RemovedItems.Cast<FileInfo>())
            {
                if (_fileOpenPickerUI.ContainsFile(removedFile.Title))
                {
                    _fileOpenPickerUI.RemoveFile(removedFile.Title);
                }
            }
        }

        /// <summary>
        /// Invoked when user removes one of the items from the Picker basket
        /// </summary>
        /// <param name="sender">The FileOpenPickerUI instance used to contain the available files.</param>
        /// <param name="e">Event data that describes the file removed.</param>
        private async void HandleFilePickerUIFileRemoved(FileOpenPickerUI sender, 
            FileRemovedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Synchronize the select items in the UI lists to remove 
                // the item that was removed from the picker's 'basket'
                var removedSelectedGridItem = 
                    FileGridView.SelectedItems.Cast<FileInfo>()
                    .FirstOrDefault(x => x.Title == e.Id);
                if (removedSelectedGridItem != null)
                {
                    FileGridView.SelectedItems.Remove(removedSelectedGridItem);
                }

                var removedSelectedListItem = 
                    FileGridView.SelectedItems.Cast<FileInfo>()
                    .FirstOrDefault(x => x.Title == e.Id);
                if (removedSelectedListItem != null)
                {
                    FileListView.SelectedItems.Remove(removedSelectedListItem);
                }
            });
        }

        /// <summary>
        /// Invoked when the "Go up" button is clicked, indicating that the user wants to pop up
        /// a level in the hierarchy of files.
        /// </summary>
        /// <param name="sender">The Button instance used to represent the "Go up" command.</param>
        /// <param name="e">Event data that describes how the button was clicked.</param>
        private void GoUpButton_Click(Object sender, RoutedEventArgs e)
        {
            // Not used in this example, since the hierarchy is fairly "flat"
        }
    }
}
