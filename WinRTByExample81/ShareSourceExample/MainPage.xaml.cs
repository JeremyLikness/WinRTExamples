using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json.Linq;
using ShareSourceExample.Common;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace ShareSourceExample
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const String SampleFile1 = "ms-appx:///Assets/SampleOne.png";
        private const String SampleFile2 = "ms-appx:///Assets/SampleTwo.png";
        private const String SampleFile3 = "ms-appx:///Assets/SampleThree.png";


        private readonly NavigationHelper _navigationHelper;
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

        public MainPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += OnNavigationHelperLoadState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void OnNavigationHelperLoadState(Object sender, LoadStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);
            SubscribeToDataTransferManager();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            RemoveSubscriptionToDataTransferManager();
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void SubscribeToDataTransferManager()
        {
            var transferManager = DataTransferManager.GetForCurrentView();
            transferManager.DataRequested += OnShareDataRequested;
            transferManager.TargetApplicationChosen += OnShareTargetApplicationChosen;
        }

        private void RemoveSubscriptionToDataTransferManager()
        {
            var transferManager = DataTransferManager.GetForCurrentView();
            transferManager.DataRequested -= OnShareDataRequested;
            transferManager.TargetApplicationChosen -= OnShareTargetApplicationChosen;
        }

        private async void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            /* SIMPLIFIED BOOK EXAMPLE
            var properties = args.Request.Data.Properties;
            properties.Title = "Title Text";
            properties.Description = "Description Text";
            properties.ContentSourceWebLink = 
                new Uri("http://winrtexamples.codeplex.com");
            */

            GetPackagePropertySelections(args.Request.Data);
            var shareTitle = args.Request.Data.Properties.Title;
            if (String.IsNullOrWhiteSpace(shareTitle))
            {
                args.Request.FailWithDisplayText("A title must be supplied for sharing.");
                return;
            }

            var atLeastOneItemIsSelected = ShareContentGrid.Children.OfType<CheckBox>().All(x => x.IsChecked != true);
            if (atLeastOneItemIsSelected)
            {
                args.Request.FailWithDisplayText("At least one type of data to share must be selected.");
                return;
            }
            try
            {
                // Request a deferral to accomodate some of the async operations that might be included 
                var deferral = args.Request.GetDeferral();
                // Perform asynchronous operation(s)
                await GetPackageShareContentSelections(args.Request.Data);
                deferral.Complete();
            }
            catch (InvalidOperationException ex)
            {
                args.Request.FailWithDisplayText("An error occurred while compuling the share data - " + ex.Message);
                return;
            }

            //dataPackage.Properties  // Custom properties
            //args.Request.Deadline
        }

        private void OnShareTargetApplicationChosen(DataTransferManager sender, TargetApplicationChosenEventArgs args)
        {
        }

        private void GetPackagePropertySelections(DataPackage dataPackage)
        {
            if (dataPackage == null) throw new ArgumentNullException("dataPackage");

            var properties = dataPackage.Properties;

            // Title
            properties.Title = SetTitleTextBox.Text;

            // Description
            if (SetDescriptionCheckBox.IsChecked == true)
            {
                properties.Description = SetDescriptionTextBox.Text;
            }

            // Logo
            if (SetLogoCheckBox.IsChecked == true)
            {
                RandomAccessStreamReference stream = null;
                if (LogoImage1RadioButton.IsChecked == true)
                {
                    stream = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/SampleOne.png"));
                }
                else if (LogoImage2RadioButton.IsChecked == true)
                {
                    stream = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/SampleTwo.png"));
                }
                else if (LogoImage3RadioButton.IsChecked == true)
                {
                    stream = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/SampleThree.png"));
                }
                if (stream != null)
                {
                    properties.Square30x30Logo = stream;
                }

                // Background Color
                var red = Convert.ToByte(SetLogoBackgroundRedTextBox.Text, 16);
                var green = Convert.ToByte(SetLogoBackgroundGreenTextBox.Text, 16);
                var blue = Convert.ToByte(SetLogoBackgroundBlueTextBox.Text, 16);
                properties.LogoBackgroundColor = new Color { A = 0xFF, R = red, G = green, B = blue };
            }

            // Content Source Web Link
            if (SetContentSourceWebLinkCheckBox.IsChecked == true)
            {
                if (Uri.IsWellFormedUriString(SetContentSourceWebLinkTextBox.Text, UriKind.RelativeOrAbsolute))
                {
                    properties.ContentSourceWebLink = new Uri(SetContentSourceWebLinkTextBox.Text);
                }
            }

            // Content Source App Link
            if (SetContentSourceAppLinkCheckBox.IsChecked == true)
            {
                if (Uri.IsWellFormedUriString(SetContentSourceAppLinkTextBox.Text, UriKind.RelativeOrAbsolute))
                {
                    properties.ContentSourceApplicationLink = new Uri(SetContentSourceAppLinkTextBox.Text);
                }
            }
        }

        private async Task GetPackageShareContentSelections(DataPackage dataPackage)
        {
            if (dataPackage == null) throw new ArgumentNullException("dataPackage");

            if (SetShareTextCheckBox.IsChecked == true)
            {
                ConfigureTextShareContent(dataPackage);
            }

            if (SetShareRtfTextCheckBox.IsChecked == true)
            {
                //ConfigureRtfShareContent(dataPackage);
            }

            if (SetShareHtmlCheckBox.IsChecked == true)
            {
                var isDelayed = SetShareHtmlUseDelayedCheckBox.IsChecked == true;
                ConfigureHtmlShareContent(dataPackage, !isDelayed);
            }
            
            if (SetShareBitmapCheckBox.IsChecked == true)
            {
                var isDelayed = SetShareBitmapUseDelayedCheckBox.IsChecked == true;
                if (!isDelayed)
                {
                    await ConfigureBitmapShareContentImmediate(dataPackage);
                }
                else
                {
                    ConfigureBitmapShareContentDelayed(dataPackage);
                }
            }

            // Note - SetUri is effectively Deprecated - see SetApplicationLink and/or SetWebLink instead
            if (SetShareLinksCheckBox.IsChecked == true)
            {
                ConfigureLinksShareContent(dataPackage);
            }

            if (SetShareFileListCheckBox.IsChecked == true)
            {
                await ConfigureImmediateStorageItemsShareContent(dataPackage);
            }

            if (SetShareCustomDataCheckBox.IsChecked == true)
            {
                var isDelayed = SetShareCustomDataUseDelayedCheckBox.IsChecked == true;
                ConfigureCustomDataShareContent(dataPackage, !isDelayed);
            }
        }

        private void ConfigureTextShareContent(DataPackage dataPackage)
        {
            /* SIMPLIFIED BOOK EXAMPLE
            dataPackage.SetText("Some text to share");
            */

            var shareText = SetShareTextTextBox.Text;
            if (String.IsNullOrWhiteSpace(shareText))
            {
                throw new InvalidOperationException("If sharing text has been selected, the text cannot be empty");
            }
            dataPackage.SetText(shareText);
        }

        // TODO - RTF Example/Support
        //private void ConfigureRtfShareContent(DataPackage dataPackage)
        //{
        //    //dataPackage.SetRtf();
        //}

        private void ConfigureHtmlShareContent(DataPackage dataPackage, Boolean isImmediate)
        {
            /* SIMPLIFIED BOOK EXAMPLE
            var htmlFragment =
                "<h1>HTML Sharing Example</h1>" +
                "<p>This is an example of HTML sharing.<p>" +
                "<img src=file1>" +
                "<img src=file2>" +
                "<img src=file3>";

            var uriForSample1 = new Uri("ms-appx:///Assets/SampleOne.png");
            var uriForSample2 = new Uri("ms-appx:///Assets/SampleTwo.png");
            var uriForSample3 = new Uri("ms-appx:///Assets/SampleThree.png");

            dataPackage.ResourceMap["file1"] =
                RandomAccessStreamReference.CreateFromUri(uriForSample1);
            dataPackage.ResourceMap["file2"] =
                RandomAccessStreamReference.CreateFromUri(uriForSample2);
            dataPackage.ResourceMap["file3"] =
                RandomAccessStreamReference.CreateFromUri(uriForSample3);

            var formattedHtmlFragment = 
                HtmlFormatHelper.CreateHtmlFormat(htmlFragment);
            dataPackage.SetHtmlFormat(formattedHtmlFragment);
            */

            var includeImages = IncludeHtmlImagesCheckBox.IsChecked == true;
            var htmlContent = "<h1>HTML Sharing Example</h1><p>This is an example of HTML sharing.<p>";
            if (includeImages)
            {
                htmlContent += "<img src=file1>";
                htmlContent += "<img src=file2>";
                htmlContent += "<img src=file3>";

                // The ResourceMap provides a tool for the receiving application to use to dereference content with relative links
                dataPackage.ResourceMap["file1"] = RandomAccessStreamReference.CreateFromUri(new Uri(SampleFile1));
                dataPackage.ResourceMap["file2"] = RandomAccessStreamReference.CreateFromUri(new Uri(SampleFile2));
                dataPackage.ResourceMap["file3"] = RandomAccessStreamReference.CreateFromUri(new Uri(SampleFile3));

                // Note - can also use RandomAccessStreamReference.CreateFromFileAsync to access user files referenced in app storage, for example
            }

            // Apply the necessary headers to ensure the content is forrmatted for Share operations.
            var formattedHtmlContent = HtmlFormatHelper.CreateHtmlFormat(htmlContent);

            if (isImmediate)
            {
                dataPackage.SetHtmlFormat(formattedHtmlContent);
            }
            else
            {
                ConfigureCallbackDataShareContent(dataPackage, StandardDataFormats.Html, () => formattedHtmlContent);
            }

            // TODO - try to add HTML previewer
            // TODO - consider adding a user-enters-a-url option
        }

        private async Task ConfigureBitmapShareContentImmediate(DataPackage dataPackage)
        {
            /* SIMPLIFIED BOOK EXAMPLE
            var bitmapUri = new Uri("ms-appx:///Assets/Logo.png");
            var streamRef = 
                RandomAccessStreamReference.CreateFromUri(bitmapUri);
            dataPackage.SetBitmap(streamRef);

            var thumbnailUri = new Uri("ms-appx:///Assets/SmallLogo.png");
            var thumbnailRef = 
                RandomAccessStreamReference.CreateFromUri(thumbnailUri);
            dataPackage.Properties.Thumbnail = thumbnailRef;

            // NOTE - This will require obtaining a deferral!
            var storageFile = 
                await StorageFile.GetFileFromApplicationUriAsync(bitmapUri);
            dataPackage.SetStorageItems(new[] { storageFile });
            */

            //var useCustomImage = UseCustomBitmapCheckBox.IsChecked == true;

            //if (useCustomImage)
            //{
            //    // TODO - Consider adding code to bring up a picker and select a single image file, then turn the result into an image AND a thumbnail
            //    // NOTE: do the fetch before hand - if will take more than 200ms to fetch the image, a delegate function approach will be required

            //    // Be sure to include thumbnails to provide a preview of large images to Target apps
            //    //dataPackage.Properties.Thumbnail = 
            //}
            //else
            {
                // Be sure to include a thumbnail
                var bitmapThumbnailUri = new Uri("ms-appx:///Assets/SmallLogo.png");
                dataPackage.Properties.Thumbnail = RandomAccessStreamReference.CreateFromUri(bitmapThumbnailUri);

                var bitmapUri = new Uri("ms-appx:///Assets/Logo.png");
                dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromUri(bitmapUri));

                // It is also a good idea to include a Storage Item along with the Bitmap
                var file = await StorageFile.GetFileFromApplicationUriAsync(bitmapUri);
                dataPackage.SetStorageItems(new[] {file});
            }

            // TODO - add UI image preview
        }

        private void ConfigureBitmapShareContentDelayed(DataPackage dataPackage)
        {
            var bitmapUri = new Uri("ms-appx:///Assets/Logo.png");
            ConfigureCallbackDataShareContent(dataPackage, StandardDataFormats.Bitmap, () => RandomAccessStreamReference.CreateFromUri(bitmapUri));
            //ConfigureCallbackDataShareContent(dataPackage, StandardDataFormats.StorageItems, async () => await StorageFile.GetFileFromApplicationUriAsync(bitmapUri));
        }

        private async Task ConfigureImmediateStorageItemsShareContent(DataPackage dataPackage)
        {
            /* SIMPLIFIED BOOK EXAMPLE
            var uriForSample1 = new Uri("ms-appx:///Assets/SampleOne.png");
            var uriForSample2 = new Uri("ms-appx:///Assets/SampleTwo.png");
            var uriForSample3 = new Uri("ms-appx:///Assets/SampleThree.png");

            // NOTE - The following async calls will require obtaining a deferral!
            var file1 = 
                await StorageFile.GetFileFromApplicationUriAsync(uriForSample1);
            var file2 = 
                await StorageFile.GetFileFromApplicationUriAsync(uriForSample2);
            var file3 = 
                await StorageFile.GetFileFromApplicationUriAsync(uriForSample3);
            var items = new[] {file1, file2, file3};
            dataPackage.SetStorageItems(items);
            */

            //if (UseCustomFileListCheckBox.IsChecked == true)
            //{
            //    // TODO - Consider supporting user file selection
            //    // NOTE - DO NOT PUT THIS HERE - GET THE RESULT OF THIS (FROM ELSEWHERE) LOADED UP HERE
            //    //var filePicker = new FileOpenPicker
            //    //{
            //    //    ViewMode = PickerViewMode.List,
            //    //    SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            //    //    FileTypeFilter = { "*" }
            //    //};
            //}
            //else
            {
                var file1 = await StorageFile.GetFileFromApplicationUriAsync(new Uri(SampleFile1));
                var file2 = await StorageFile.GetFileFromApplicationUriAsync(new Uri(SampleFile2));
                var file3 = await StorageFile.GetFileFromApplicationUriAsync(new Uri(SampleFile3));
                var fileList = new[] {file1, file2, file3};
                dataPackage.SetStorageItems(fileList);
            }
        }

        private void ConfigureLinksShareContent(DataPackage dataPackage)
        {
            var appUri = new Uri("winrt-by-example:contentlaunch");
            dataPackage.SetApplicationLink(appUri);

            var webUri = new Uri("http://winrtexamples.codeplex.com");
            dataPackage.SetWebLink(webUri);
        }

        private const String CustomPersonSchemaName = "http://schema.org/Person";

        private void ConfigureCustomDataShareContent(DataPackage dataPackage, Boolean isImmediate)
        {
            /* SIMPLIFIED BOOK EXAMPLE
            dataPackage.SetData("http://schema.org/Person", personJson);
            dataPackage.SetData("myCustomSchema", customSchemaObject);
            */

            var content = new JObject
                          {
                              new JProperty("type", "http://schema.org/Person"),
                              new JProperty("properties", new JObject(
                                  new JProperty("name", "John Garland"),
                                  new JProperty("givenName", "John"),
                                  new JProperty("familyName", "Garland"),
                                  new JProperty("gender", "male"),
                                  new JProperty("address", new JObject(
                                      new JProperty("type", "http://schema.org/PostalAddress"),
                                      new JProperty("properties", new JObject(
                                          new JProperty("addressLocality", "Nashua"),
                                          new JProperty("addressRegion", "New Hampshire"),
                                          new JProperty("postalCode", "03062"),
                                          new JProperty("addressCountry", "USA")
                                          )))),
                                  new JProperty("worksFor", "Wintellect, LLC"),
                                  new JProperty("jobTitle", "Senior Consultant")
                                  ))
                          };
            var personJson = content.ToString();
            if (isImmediate)
            {
                dataPackage.SetData(CustomPersonSchemaName, personJson);
            }
            else
            {
                ConfigureCallbackDataShareContent(dataPackage, CustomPersonSchemaName, () => personJson);
            }

            // Some other examples
            //dataPackage.SetData("AnsiText", "Hello World AnsiText");
            //dataPackage.SetData("OEMText", "Hello World OEMText");
        }

        private void ConfigureCallbackDataShareContent(DataPackage dataPackage, String format, Func<Object> valueRetrievalCallback)
        {
            // Provide a custom callback that simulates a long-running operation by introducing a 10-second delay 
            // before fetching the data to return with the supplied callback action.
            dataPackage.SetDataProvider(format, async request => await ForcedDelayDataProviderCallback(request, valueRetrievalCallback));
        }

        private async Task ForcedDelayDataProviderCallback(DataProviderRequest request, Func<Object> valueRetrievalCallback)
        {
            var deferral = request.GetDeferral();
            await Task.Delay(TimeSpan.FromSeconds(10));
            request.SetData(valueRetrievalCallback());
            deferral.Complete();
        }

        //private void ConfigureCallbackDataShareContent(DataPackage dataPackage, String format, Func<Task<Object>> valueRetrievalCallback)
        //{
        //    // Provide a custom callback that simulates a long-running operation by introducing a 10-second delay 
        //    // before fetching the data to return with the supplied callback action.
        //    dataPackage.SetDataProvider(format, async request => await ForcedDelayDataProviderCallback(request, valueRetrievalCallback));
        //}

        //private async Task ForcedDelayDataProviderCallback(DataProviderRequest request, Func<Task<Object>> valueRetrievalCallback)
        //{
        //    var deferral = request.GetDeferral();
        //    await Task.Delay(TimeSpan.FromSeconds(10));
        //    request.SetData(await valueRetrievalCallback());
        //    deferral.Complete();
        //}
    }
} 