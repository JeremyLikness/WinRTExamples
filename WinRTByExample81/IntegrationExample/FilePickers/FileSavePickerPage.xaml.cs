using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Contacts;
using Windows.Storage.Pickers.Provider;
using Windows.UI.Xaml.Controls;
using IntegrationExample.Common;
using IntegrationExample.Data;

namespace IntegrationExample
{
    /// <summary>
    /// This page displays files owned by the application so that the user can grant another application
    /// access to them.
    /// </summary>
    public sealed partial class FileSavePickerPage : Page
    {
        /// <summary>
        /// Files are added to or removed from the Windows UI to let Windows know what has been selected.
        /// </summary>
        private FileSavePickerUI _fileSavePickerUI;
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSavePickerPage"/> class.
        /// </summary>
        public FileSavePickerPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Used to set up the content to be displayed and connect to the containing File Open Picker
        /// </summary>
        /// <param name="fileSavePickerUI">The file save picker UI element.</param>
        public void Initialize(FileSavePickerUI fileSavePickerUI)
        {
            if (fileSavePickerUI == null) throw new ArgumentNullException("fileSavePickerUI");
            
            // Tie into the containing File Picker object
            _fileSavePickerUI = fileSavePickerUI;
            _fileSavePickerUI.TargetFileRequested += HandleTargetFileRequested;

            // Initialize the ViewModel
            DefaultViewModel["CanGoUp"] = false;
            DefaultViewModel["Contacts"] = null;
            DefaultViewModel["SelectedContact"] = null;

            // Load the contact info
            LoadContacts();
        }

        private void LoadContacts()
        {
            // Fetch the data and set it to the View Model
            var sampleDataGroups = AppSampleData.Current.SampleData.Groups;
            var allContacts =
                sampleDataGroups.SelectMany(group => group.Items)
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .ToList();
            DefaultViewModel["Contacts"] = allContacts;
        }

        private async void HandleSelectedContactChanged(Object sender, SelectionChangedEventArgs e)
        {
            var files = new List<FileInfo>();
            if (DefaultViewModel["SelectedContact"] != null)
            {
                var currentContact = (Contact)DefaultViewModel["SelectedContact"];
                files = await currentContact.GetRelatedFiles();
            }
            DefaultViewModel["Files"] = files;
        }

        /// <summary>
        /// Invoked when a file in the list is selected.
        /// </summary>
        /// <param name="sender">The GridView instance used to display the available files.</param>
        /// <param name="e">Event data that describes how the selection changed.</param>
        private void HandleSelectedFileChanged(Object sender, 
            SelectionChangedEventArgs e)
        {
            // The user selected a pre-existing file. 
            // Use its name as for overwriting.
            var selectedFile = ((ListViewBase)sender).SelectedItem as FileInfo;
            if (selectedFile != null)
            {
                _fileSavePickerUI.TrySetFileName(selectedFile.File.Name);
            }
        }

        /// <summary>
        /// Invoked when the user presses the "Save" button in the Picker
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="TargetFileRequestedEventArgs"/> instance containing the event data.</param>
        private async void HandleTargetFileRequested(FileSavePickerUI sender, 
            TargetFileRequestedEventArgs args)
        {
            var currentContact = 
                DefaultViewModel["SelectedContact"] as Contact;
            if (currentContact == null) return;

            // Requesting a deferral allows the app to call an 
            // asynchronous method and complete the request.
            var deferral = args.Request.GetDeferral();

            // Get the target file based on the currently selected contact
            args.Request.TargetFile 
                = await currentContact.SaveRelatedFile(sender.FileName);
            
            // Complete the deferral to let the Picker know 
            // that the request processing has finished
            deferral.Complete();
        }
    }
}
