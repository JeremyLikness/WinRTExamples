using System;
using System.Linq;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Contacts.Provider;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using IntegrationExample.Common;

namespace IntegrationExample
{
    /// <summary>
    /// This page displays files owned by the application so that the user can grant another application
    /// access to them.
    /// </summary>
    public sealed partial class ContactPickerPage : Page
    {
        /// <summary>
        /// Files are added to or removed from the Windows UI to let Windows know what has been selected.
        /// </summary>
        private ContactPickerUI _contactPickerUI;
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
        public ContactPickerPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when another application wants to open files from this application.
        /// </summary>
        /// <param name="contactPickerUI">The contact picker user interface element.</param>
        public void Initialize(ContactPickerUI contactPickerUI)
        {
            if (contactPickerUI == null) throw new ArgumentNullException("contactPickerUI");

            // Tie into the containing File Picker object
            _contactPickerUI = contactPickerUI;
            _contactPickerUI.ContactRemoved += HandleContactPickerUIContactRemoved;

            // Initialize the ViewModel
            DefaultViewModel["CanGoUp"] = false;
            DefaultViewModel["ContactGroups"] = null;
            DefaultViewModel["SelectedContact"] = null;

            // Fetch the data and set it to the View Model
            var sampleDataGroups = AppSampleData.Current.SampleData.Groups
                .OrderBy(x => x.Key)
                .ToList();
            DefaultViewModel["ContactGroups"] = sampleDataGroups;
        }

        /// <summary>
        /// Invoked when the selected collection of files changes.
        /// </summary>
        /// <param name="sender">The GridView instance used to display the available files.</param>
        /// <param name="e">Event data that describes how the selection changed.</param>
        private void HandleSelectedContactChanged(Object sender, SelectionChangedEventArgs e)
        {
            // Update the picker 'basket' to remove any newly removed items
            foreach (var removedItem in e.RemovedItems.Cast<Contact>())
            {
                if (_contactPickerUI.ContainsContact(removedItem.Id))
                {
                    _contactPickerUI.RemoveContact(removedItem.Id);
                }
            }

            // Update the picker 'basket' with the newly selected items
            foreach (var addedItem in e.AddedItems.Cast<Contact>())
            {
                if (!_contactPickerUI.ContainsContact(addedItem.Id))
                {
                    _contactPickerUI.AddContact(addedItem);
                }
            }
        }

        /// <summary>
        /// Invoked when user removes one of the items from the Picker basket
        /// </summary>
        /// <param name="sender">The FileOpenPickerUI instance used to contain the available files.</param>
        /// <param name="e">Event data that describes the file removed.</param>
        private async void HandleContactPickerUIContactRemoved(ContactPickerUI sender, ContactRemovedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Synchronize the select items in the UI lists to remove hte item that was removed from the picker's 'basket'
                var removedSelectedGridItem = ContactGridView.SelectedItems.Cast<Contact>().FirstOrDefault(x => x.Id == e.Id);
                if (removedSelectedGridItem != null)
                {
                    ContactGridView.SelectedItems.Remove(removedSelectedGridItem);
                }
            });
        }
    }
}
