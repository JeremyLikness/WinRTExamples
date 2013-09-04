// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   An empty page that can be used on its own or navigated to within a Frame.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FilePickerExample
{
    using System;
    using System.IO;
    using System.Linq;

    using Windows.Storage;
    using Windows.Storage.AccessCache;
    using Windows.Storage.Pickers;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// The last used file.
        /// </summary>
        private const string LastUsedFile = "LastUsedFile";

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RebuildEntries();
        }

        /// <summary>
        /// The set most recently used file.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        private static void SetMostRecentlyUsedFile(IStorageItem file)
        {
            StorageApplicationPermissions.FutureAccessList.AddOrReplace(LastUsedFile, file);
        }

        /// <summary>
        /// The rebuild entries.
        /// </summary>
        private void RebuildEntries()
        {
            var entries = StorageApplicationPermissions.MostRecentlyUsedList.Entries;
            var recentList = entries.Select(item =>
                new RecentItem
                {
                    Name = item.Metadata,
                    Token = item.Token
                }).ToList();
            RecentFiles.ItemsSource = recentList;
        }

        /// <summary>
        /// The pick button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void PickButton_OnClick(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            picker.FileTypeFilter.Add(".txt");
            var file = await picker.PickSingleFileAsync();
            if (file == null)
            {
                return;
            }

            var text = await FileIO.ReadTextAsync(file);
            TextDisplay.Text = text;

            StorageApplicationPermissions.MostRecentlyUsedList.Add(file, file.Name);
            SetMostRecentlyUsedFile(file);
            RebuildEntries();
        }

        /// <summary>
        /// The recent files_ on selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void RecentFiles_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var recentItem = RecentFiles.SelectedItem as RecentItem;
            if (recentItem == null)
            {
                return;
            }

            try
            {
                var file = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(recentItem.Token);
                SetMostRecentlyUsedFile(file);
                var text = await FileIO.ReadTextAsync(file);
                TextDisplay.Text = text;
            }
            catch (FileNotFoundException)
            {
                TextDisplay.Text = "** The file no longer exists. **";
                StorageApplicationPermissions.MostRecentlyUsedList.Remove(recentItem.Token);
                RebuildEntries();
            }
        }

        /// <summary>
        /// The last button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void LastButton_OnClick(
            object sender,
            RoutedEventArgs e)
        {
            if (!StorageApplicationPermissions.FutureAccessList
                .ContainsItem(LastUsedFile))
            {
                return;
            }

            try
            {
                var file = await StorageApplicationPermissions.FutureAccessList
                    .GetFileAsync(LastUsedFile);
                var text = await FileIO.ReadTextAsync(file);
                TextDisplay.Text = text;
            }
            catch (FileNotFoundException)
            {
                StorageApplicationPermissions.FutureAccessList
                    .Remove(LastUsedFile);
            }
        }
    }
}
