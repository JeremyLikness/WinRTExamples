// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Example of using a query and bulk access to list music files
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QueryMusicLibrary
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Windows.Storage;
    using Windows.Storage.BulkAccess;
    using Windows.Storage.FileProperties;
    using Windows.Storage.Search;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += this.MainPageLoaded;
        }

        /// <summary>
        /// The main page loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void MainPageLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var queryOptions = new QueryOptions(
                CommonFileQuery.OrderByTitle, 
                new List<string> { ".jpg", ".gif", ".tif", ".png" })
                                    {                                       
                                        FolderDepth =
                                            FolderDepth.Deep,
                                        IndexerOption =
                                            IndexerOption
                                            .UseIndexerWhenAvailable
                                    };
            queryOptions.SetThumbnailPrefetch(ThumbnailMode.PicturesView, 150, ThumbnailOptions.ResizeThumbnail);
            var query = KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(queryOptions);
            var access = new FileInformationFactory(query, ThumbnailMode.PicturesView);
            var fileList = access.GetVirtualizedFilesVector();
            MainList.ItemsSource = fileList;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
