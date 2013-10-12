using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Newtonsoft.Json.Linq;
using ShareTargetExample.Common;
using System.Threading.Tasks;

// The Share Target Contract item template is documented at http://go.microsoft.com/fwlink/?LinkId=234241

namespace ShareTargetExample
{
    /// <summary>
    /// This page allows other applications to share content through this application.
    /// </summary>
    public sealed partial class ShareTargetPage : Page
    {
        private const String CustomPersonSchemaName = "http://schema.org/Person";

        /// <summary>
        /// Provides a channel to communicate with Windows about the sharing operation.
        /// </summary>
        private ShareOperation _shareOperation;
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

        public ShareTargetPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when another application wants to share content through this application.
        /// </summary>
        /// <param name="e">Activation data used to coordinate the process with Windows.</param>
        public void Activate(ShareTargetActivatedEventArgs e)
        {
            _shareOperation = e.ShareOperation;

            InitializeViewModel();

            Window.Current.Content = this;
            Window.Current.Activate();

            // Load Image Properties asynchronousely now that the panel is ready
            ShowSharedImageProperties();

            // Load any shared data asynchronousely now that the panel is ready
            ShowSharedContent();
        }

        private void InitializeViewModel()
        {
            var shareProperties = _shareOperation.Data.Properties;

            // Communicate metadata about the shared content through the view model
            // Sharing metadata
            DefaultViewModel["Title"] = shareProperties.Title;
            DefaultViewModel["Description"] = shareProperties.Description;
            DefaultViewModel["ApplicationListingUri"] = shareProperties.ApplicationListingUri;
            DefaultViewModel["ApplicationName"] = shareProperties.ApplicationName;
            DefaultViewModel["ContentSourceApplicationLink"] = shareProperties.ContentSourceApplicationLink;
            DefaultViewModel["ContentSourceWebLink"] = shareProperties.ContentSourceWebLink;

            DefaultViewModel["LaunchedWithQuickLink"] = !String.IsNullOrWhiteSpace(_shareOperation.QuickLinkId);
            DefaultViewModel["QuickLinkId"] = _shareOperation.QuickLinkId;

            DefaultViewModel["Logo"] = new BitmapImage();
            DefaultViewModel["ShowLogoImage"] = false;
            DefaultViewModel["LogoBackground"] = new SolidColorBrush(shareProperties.LogoBackgroundColor);

            DefaultViewModel["ThumbnailImage"] = new BitmapImage();
            DefaultViewModel["ShowThumbnailImage"] = false;

            // Sharing content
            DefaultViewModel["SharedText"] = null;
            DefaultViewModel["IsTextShared"] = false;
            DefaultViewModel["SharedRtf"] = null;
            DefaultViewModel["IsRtfShared"] = false;
            DefaultViewModel["SharedHtml"] = "<html></html>";
            DefaultViewModel["IsHtmlShared"] = false;
            DefaultViewModel["IsHtmlLoading"] = false;
            DefaultViewModel["SharedBitmap"] = new BitmapImage();
            DefaultViewModel["IsBitmapShared"] = false;
            DefaultViewModel["IsBitmapLoading"] = false;
            DefaultViewModel["SharedApplicationLink"] = null;
            DefaultViewModel["IsApplicationLinkShared"] = false;
            DefaultViewModel["SharedWebLink"] = null;
            DefaultViewModel["IsWebLinkShared"] = false;
            DefaultViewModel["SharedStorageItems"] = new String[]{};
            DefaultViewModel["IsStorageItemsShared"] = false;
            DefaultViewModel["SharedCustomItem"] = null;
            DefaultViewModel["IsCustomItemShared"] = false;
            DefaultViewModel["IsCustomItemLoading"] = false;

            // Controls for updating processing status
            DefaultViewModel["ProcessingSharedData"] = false;
            DefaultViewModel["UseQuickLink"] = false;
            DefaultViewModel["QuickLinkTitle"] = String.Empty;
            DefaultViewModel["QuickLinkTag"] = String.Empty;
            DefaultViewModel["ErrorMessage"] = String.Empty;
        }

        private async void ShowSharedImageProperties()
        {
            var shareProperties = _shareOperation.Data.Properties;

            // Update the shared content's logo image in the background
            if (shareProperties.Square30x30Logo != null)
            {
                var logoStream = await shareProperties.Square30x30Logo.OpenReadAsync();
                ((BitmapImage)DefaultViewModel["Logo"]).SetSource(logoStream);
                DefaultViewModel["ShowLogoImage"] = true;
            }

            // Update the shared content's thumbnail image (if provided) in the background
            if (shareProperties.Thumbnail != null)
            {
                var stream = await shareProperties.Thumbnail.OpenReadAsync();
                ((BitmapImage)DefaultViewModel["ThumbnailImage"]).SetSource(stream);
                DefaultViewModel["ShowThumbnailImage"] = true;
            }
        }

        private void ShowSharedContent()
        {
            // Let Windows know that this app is starting to fetch data
            _shareOperation.ReportStarted();
            DefaultViewModel["ProcessingSharedData"] = true;

            try
            {
                var shareData = _shareOperation.Data;

                var showContentTaskList = new List<Task>();
                if (shareData.Contains(StandardDataFormats.Text))
                {
                    var task = ShowSharedText(shareData);
                    showContentTaskList.Add(task);
                }

                if (shareData.Contains(StandardDataFormats.Rtf))
                {
                    // Not Implemented
                    //ShowSharedRtf(shareData);
                }

                if (shareData.Contains(StandardDataFormats.Html))
                {
                    var task = ShowSharedHtml(shareData);
                    showContentTaskList.Add(task);
                }

                if (shareData.Contains(StandardDataFormats.Bitmap))
                {
                    var task = ShowSharedBitmap(shareData);
                    showContentTaskList.Add(task);
                }

                // NOTE - SetUri is effectively Deprecated - see SetApplicationLink and/or SetWebLink instead
                if (shareData.Contains(StandardDataFormats.ApplicationLink))
                {
                    var task = ShowSharedAppLink(shareData);
                    showContentTaskList.Add(task);
                }
                if (shareData.Contains(StandardDataFormats.WebLink))
                {
                    var task = ShowSharedWebLink(shareData);
                    showContentTaskList.Add(task);
                }

                if (shareData.Contains(StandardDataFormats.StorageItems))
                {
                    var task = ShowSharedStorageItems(shareData);
                    showContentTaskList.Add(task);
                }

                if (shareData.Contains(CustomPersonSchemaName))
                {
                    var task = ShowSharedCustomPerson(shareData);
                    showContentTaskList.Add(task);
                }

                // Admittedly a bit unusual to be doing this "task stuff"; most apps will just bring in their "favorite" data format
                // and signal Retrieved/Complete right there...since this is more of a "sampler", the Task API is used to wait for 
                // all of the retrives to finish and then signal that the data txfer is complete, and update the UI elements accordingly.
                Task.WhenAll(showContentTaskList).ContinueWith(task =>
                                                                {
                                                                    _shareOperation.ReportDataRetrieved();
                                                                    DefaultViewModel["ProcessingSharedData"] = false;
                                                                });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Share data retrieval error: " + e);
                _shareOperation.ReportError("An error occurred while retrieving the shared data.");
            }
        }

        private async Task ShowSharedText(DataPackageView shareData)
        {
            DefaultViewModel["IsTextShared"] = true;
            DefaultViewModel["SharedText"] = await shareData.GetTextAsync();
        }

        //private void ShowSharedRtf(DataPackageView shareData)
        //{
        //    //throw new NotImplementedException();
        //}

        private async Task ShowSharedHtml(DataPackageView shareData)
        {
            DefaultViewModel["IsHtmlShared"] = true;
            DefaultViewModel["IsHtmlLoading"] = true;

            var sharedHtmlContent = await shareData.GetHtmlFormatAsync();
            if (!String.IsNullOrEmpty(sharedHtmlContent))
            {
                // Convert the shared content that contains extra header data into just the working fragment
                var sharedHtmlFragment = HtmlFormatHelper.GetStaticFragment(sharedHtmlContent);

                // Reconcile any resource-mapped image references by locating the named reference in the HTML
                // and then replacing the image reference with an inline base-64 encoded image entry that is 
                // obtained by processing the named entry in the Resource Map.
                var sharedResourceMap = await shareData.GetResourceMapAsync();
                foreach (var resource in sharedResourceMap)
                {
                    var mappedImageElement = String.Format("<img src={0}>", resource.Key);
                    var replacementImageBase64 = await resource.Value.Base64EncodeContent();
                    var replacementImageElement = String.Format("<img src='data:image/png;base64,{0}' />", replacementImageBase64);
                    var imageIndex = sharedHtmlFragment.IndexOf(mappedImageElement, StringComparison.Ordinal);
                    while (imageIndex >= 0)
                    {
                        sharedHtmlFragment = sharedHtmlFragment.Remove(imageIndex, mappedImageElement.Length);
                        sharedHtmlFragment = sharedHtmlFragment.Insert(imageIndex, replacementImageElement);
                        imageIndex = sharedHtmlFragment.IndexOf(mappedImageElement, StringComparison.Ordinal);
                    }
                }

                DefaultViewModel["IsHtmlLoading"] = false;
                DefaultViewModel["SharedHtml"] = sharedHtmlFragment;
            }
        }

        private async Task ShowSharedBitmap(DataPackageView shareData)
        {
            DefaultViewModel["IsBitmapShared"] = true;
            DefaultViewModel["IsBitmapLoading"] = true;

            var bitmapStreamRef = await shareData.GetBitmapAsync();
            var bitmapStream = await bitmapStreamRef.OpenReadAsync();
            DefaultViewModel["IsBitmapLoading"] = false;
            ((BitmapImage)DefaultViewModel["SharedBitmap"]).SetSource(bitmapStream);
        }

        private async Task ShowSharedAppLink(DataPackageView shareData)
        {
            DefaultViewModel["IsApplicationLinkShared"] = true;
            DefaultViewModel["SharedApplicationLink"] 
                = await shareData.GetApplicationLinkAsync();
        }

        private async Task ShowSharedWebLink(DataPackageView shareData)
        {
            DefaultViewModel["IsWebLinkShared"] = true;
            DefaultViewModel["SharedWebLink"] 
                = await shareData.GetWebLinkAsync();
        }

        private async Task ShowSharedStorageItems(DataPackageView shareData)
        {
            DefaultViewModel["IsStorageItemsShared"] = true;
            var storageItems = await shareData.GetStorageItemsAsync();
            DefaultViewModel["SharedStorageItems"] = 
                storageItems.Select(x => x.Name).ToList();
        }

        private async Task ShowSharedCustomPerson(DataPackageView shareData)
        {
            DefaultViewModel["IsCustomItemShared"] = true;
            DefaultViewModel["IsCustomItemLoading"] = true;
            var sharedPersonObject 
                = await shareData.GetDataAsync(CustomPersonSchemaName);
            var personJsonObject = JObject.Parse(sharedPersonObject.ToString());
            DefaultViewModel["IsCustomItemLoading"] = false;
            DefaultViewModel["SharedCustomItem"] = personJsonObject.ToString();
        }

        private async void OnReportSuccessClick(Object sender, RoutedEventArgs e)
        {
            var useQuickLink = (Boolean)(DefaultViewModel["UseQuickLink"] ?? false);
            if (useQuickLink)
            {
                // Make sure a thumbnail is available
                if (_shareOperation.Data.Properties.Square30x30Logo == null) return;
                var quickLinkThumbnail = RandomAccessStreamReference.CreateFromStream(await _shareOperation.Data.Properties.Square30x30Logo.OpenReadAsync());

                // Make sure a title is available
                var quickLinkTitle = (DefaultViewModel["QuickLinkTitle"] ?? String.Empty).ToString();
                if (String.IsNullOrWhiteSpace(quickLinkTitle)) return;

                var quickLinkTag = (DefaultViewModel["QuickLinkTag"] ?? String.Empty).ToString();
                if (String.IsNullOrWhiteSpace(quickLinkTag)) return;

                var quickLink = new QuickLink
                {
                    SupportedFileTypes = { "*" },
                    Thumbnail = quickLinkThumbnail,
                    Title = quickLinkTitle,
                    Id = quickLinkTag,
                };

                // Just reuse the current set of data formats, though it could be selective 
                // based on the context of how the id value will be used.
                var quickLinkFormats = _shareOperation.Data.AvailableFormats;
                foreach (var item in quickLinkFormats)
                {
                    quickLink.SupportedDataFormats.Add(item);
                }

                _shareOperation.ReportCompleted(quickLink);
            }
            else
            {
                // Close the UI and inform Windows that the sharing completed succesfully
                _shareOperation.ReportCompleted();
            }
        }

        private void OnReportErrorClick(Object sender, RoutedEventArgs e)
        {
            var errorMessage = (DefaultViewModel["ErrorMessage"] ?? String.Empty).ToString();
            if (!String.IsNullOrWhiteSpace(errorMessage))
            {
                _shareOperation.ReportError(errorMessage);
            }
        }

        private void OnClickRemoveQuickLink(Object sender, RoutedEventArgs e)
        {
            _shareOperation.RemoveThisQuickLink();
        }
    }

    public static partial class Extensions
    {
        public static async Task<String> Base64EncodeContent(this RandomAccessStreamReference streamReference)
        {
            using (IRandomAccessStreamWithContentType stream = await streamReference.OpenReadAsync())
            {
                var readStream = stream.AsStreamForRead();
                var bytes = new byte[stream.Size];
                readStream.Read(bytes, 0, (Int32)stream.Size);
                var result = Convert.ToBase64String(bytes);
                return result;
            }
        }
    }
}
