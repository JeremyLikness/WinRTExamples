using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Printing;
using PrintingAndScanningExample.Annotations;

namespace PrintingAndScanningExample
{
    public class PrintHelper
    {
        #region Fields

        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        private readonly PrintDocument _printDocument;
        private readonly CustomPrintOptionsHelper _customPrintOptionsHelper;

        private Func<IEnumerable<PictureModel>> _pictureProvider;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintHelper"/> class.
        /// </summary>
        public PrintHelper()
        {
            // Set up the Print Document
            _printDocument = new PrintDocument();

            // This handler for the Paginate event is raised to request the count of preview pages
            _printDocument.Paginate += HandlePagination;

            // This requests/receives a specific print preview page
            _printDocument.GetPreviewPage += HandleGetPreviewPage;

            // This is called when the system is ready to start printing and provides the final pages
            _printDocument.AddPages += HandleAddPages;

            // Set up the custom options helper
            _customPrintOptionsHelper = new CustomPrintOptionsHelper();

            // Event raised when a change is made to the the custom print option 
            _customPrintOptionsHelper.CustomPrintOptionChanged += (sender, args) =>
            {
                // Called to inform that the custom template setting has changed
                // Refresh all of teh content by invalidating the current preview
                // This has to happen on the UI thread, though the call is originating elsewhere.
                _syncContext.Post(x => _printDocument.InvalidatePreview(), null);
            };
        } 

        #endregion

        #region Public Interface

        /// <summary>
        /// Attaches to the printing process and configures the various event handlers.
        /// </summary>
        /// <param name="pictureProvider">A callback function to use to retrieve the pictures to be printed.</param>
        /// <exception cref="System.ArgumentNullException">pictureCollection</exception>
        public void ConfigurePrinting([NotNull] Func<IEnumerable<PictureModel>> pictureProvider)
        {
            if (pictureProvider == null) throw new ArgumentNullException("pictureProvider");
            _pictureProvider = pictureProvider;

            // Actually connect to the PrintManager and let it know that the app now supports printing
            // If this handler is declared, printers are shown when the Device Charm is
            // invoked and "Printing" is selected.  Put another way, when there is a handler, 
            // there is an expectation that printing is currently possible.
            // Important note: If this is handler is subscribed to twice, an exception is thrown
            var printManager = PrintManager.GetForCurrentView();
            printManager.PrintTaskRequested += HandlePrintTaskRequested;

            //try
            //{
            //    printManager.PrintTaskRequested += HandlePrintTaskRequested;
            //}
            //catch (Exception e)
            //{
            //    System.Diagnostics.Debug.WriteLine("Double-Tap: {0}",e);
            //}
        }

        public void DetachPrinting()
        {
            // Detach from the PrintManager's PrintTaskRequested event to remove the expectation that printing is available
            var printManager = PrintManager.GetForCurrentView();
            printManager.PrintTaskRequested -= HandlePrintTaskRequested;
        }

        public async Task ShowPrintPreview()
        {
            await PrintManager.ShowPrintUIAsync();
        } 

        #endregion
        
        #region Print Task (Job) Configuration

        private void HandlePrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            // Called when printing is first requested (Device Charm / Printing or programmatically)
            Debug.WriteLine("PrintTask Requested");

            // Create the task, which includes a name and a callback handler for what to do when a printer is actually selected
            var printTask = args.Request.CreatePrintTask("Windows Runtime By Example Printing", HandlePrintTaskSourceRequested);

            // Subscribe to lifecycle events for the current task/job 
            // Note - Unsubscribe in "completed", which will always be called
            printTask.Previewing += HandlePrintTaskPreviewing;  // Called before pages are requested for preview
            printTask.Submitting += HandlePrintTaskSubmitting;  // Called when the user hits "print"
            printTask.Progressing += HandlePrintTaskProgressing;  // Called once for each page that is printed
            printTask.Completed += HandlePrintTaskCompleted;  // Called after all pages are printed

            // Define the list of print options to be shown for the current task/job
            // Indicate which standard options should be included/not included
            var previewDisplayedOptions = printTask.Options.DisplayedOptions;
            previewDisplayedOptions.Clear();
            previewDisplayedOptions.Add(StandardPrintTaskOptions.Copies);
            previewDisplayedOptions.Add(StandardPrintTaskOptions.Orientation);

            // Add/incorporate the app's custom print options into the list of print options, above
            var customOptions = _customPrintOptionsHelper.ConfigureCustomOptions(printTask.Options);
            foreach (var customOption in customOptions)
            {
                previewDisplayedOptions.Add(customOption.OptionId);
            }
            
            // If the work done to pull together the PrintTask is asynchronous, need to work with a deferral
            // var deferral = args.Request.GetDeferral();
            // Work that contains async operations goes here
            // deferral.Complete();
        }

        private void HandlePrintTaskSourceRequested(PrintTaskSourceRequestedArgs args)
        {
            Debug.WriteLine("PrintTask Source Requested");
            // Called when a printer device is selected
            var deferral = args.GetDeferral();

            // Set the document source for the current print request.  This MUST happen on the UI thread.
            _syncContext.Post(x =>
            {
                args.SetSource(_printDocument.DocumentSource);

                // Complete the deferral to indicate that all related async operations have completed
                deferral.Complete();
            }, null);
        }

        #endregion

        #region Print Task (Job) Lifecycle Events
        
        private void HandlePrintTaskPreviewing(PrintTask task, Object args)
        {
            Debug.WriteLine("PrintTask Previewing - {0} ", task.Properties.Title);
        }

        private void HandlePrintTaskSubmitting(PrintTask task, Object args)
        {
            Debug.WriteLine("PrintTask Submitting - {0}", task.Properties.Title);
        }

        private void HandlePrintTaskProgressing(PrintTask task, PrintTaskProgressingEventArgs args)
        {
            
            Debug.WriteLine("PrintTask Progressing - {0}, Pages Printed = {1}", task.Properties.Title, args.DocumentPageCount);
        }

        private void HandlePrintTaskCompleted(PrintTask task, PrintTaskCompletedEventArgs args)
        {
            Debug.WriteLine("PrintTask Completed - {0}, Status = {1}.", task.Properties.Title, args.Completion);
            //args.Completion == PrintTaskCompletion.Submitted PrintTaskCompletion.Failed PrintTaskCompletion.Canceled  PrintTaskCompletion.Abandoned

            // Unsubscribe from the lifecycle events to prevent the task from being leaked
            task.Previewing -= HandlePrintTaskPreviewing;
            task.Submitting -= HandlePrintTaskSubmitting;
            task.Progressing -= HandlePrintTaskProgressing;
            task.Completed -= HandlePrintTaskCompleted;
        }

        #endregion

        #region Print Document Content to be Printed/Displayed

        private class PreviewOptions
        {
            public String PageTitle { get; set; }
            public PrintLayout SelectedLayout { get; set; }
            public PreviewTypeOption PreviewType { get; set; }
        }

        private readonly PreviewOptions _currentPreviewOptions = new PreviewOptions();

        private void HandlePagination(Object sender, PaginateEventArgs args)
        {
            // Given the curren toptions, determine the number of pages to be printed
            Debug.WriteLine("PrintDocument - Pagination");

            var printDocument = (PrintDocument)sender;
            var printOptions = args.PrintTaskOptions;

            // Record the current options driving the current pagingation calculations
            _currentPreviewOptions.PageTitle = _customPrintOptionsHelper.GetPageTitle(printOptions);
            _currentPreviewOptions.SelectedLayout = _customPrintOptionsHelper.GetSelectedPrintLayout(printOptions);
            _currentPreviewOptions.PreviewType = _customPrintOptionsHelper.GetSelectedPreviewType(printOptions);

            var picturesPerPage = _currentPreviewOptions.SelectedLayout.PicturesPerPage;
            var picturesToPrint = _pictureProvider();

            // Calculate the total page count based on number of pictures per page and total number of selected pictures
            var totalPageCount = (Int32)Math.Ceiling(picturesToPrint.Count() / (Double)picturesPerPage);

            // Set the page count to be intermediate if the number of pages is not 100% known at this point, Final if it is certain
            printDocument.SetPreviewPageCount(totalPageCount, PreviewPageCountType.Intermediate);

            // TODO - look into dumping out (debug?) these values (or optionally printing them?)
            // printOptions.ColorMode == // Greyscale, etc.
            // printOptions.Orientation == PrintOrientation.Default
            // printOptions.PrintQuality == 
            // printOptions.Binding == 
            // printOptions.Collation
            // printOptions.Duplex
            // printOptions.  etc, etc

            // Get the page description
            // var previewPageNumber = args.CurrentPreviewPageNumber;
            // var pageDescription = printOptions.GetPageDescription(previewPageNumber);
            // pageDescription.PageSize // .ImageableRect .DpiX//, DpiY
        }

        private void HandleGetPreviewPage(Object sender, GetPreviewPageEventArgs args)
        {
            // Provide the print preview content for the requested page
            Debug.WriteLine("PrintDocument - Get Preview Page - Page {0}", args.PageNumber);

            var printDocument = (PrintDocument)sender;
            var currentPageNumber = args.PageNumber;

            // Get the visual to be displayed for the currently requested page
            var pageVisual = BuildPicturePage(
                currentPageNumber,
                _currentPreviewOptions.SelectedLayout, 
                _currentPreviewOptions.PreviewType == PreviewTypeOption.Thumbnails, 
                _currentPreviewOptions.PageTitle);
            printDocument.SetPreviewPage(currentPageNumber, pageVisual);
        }

        private void HandleAddPages(Object sender, AddPagesEventArgs args)
        {
            // Build the collection of pages to display and pass them to the print document
            Debug.WriteLine("PrintDocument - AddPages");

            var printDocument = (PrintDocument)sender;
            var options = args.PrintTaskOptions;

            // Get the page title selection
            var pageTitle = _customPrintOptionsHelper.GetPageTitle(args.PrintTaskOptions);
            var picturesToPrint = _pictureProvider();

            // Determine the value of the layout template setting
            var selectedLayout = _customPrintOptionsHelper.GetSelectedPrintLayout(options);

            // args.PrintTaskOptions.GetPageDescription(); DpiX, DpiY, ImageableRect, PageSize
            var pageCount = Math.Ceiling(picturesToPrint.Count() / (Double)selectedLayout.PicturesPerPage);
            for (var i = 1; i <= pageCount; i++)
            {
                var pageVisual = BuildPicturePage(i, selectedLayout, false, pageTitle);
                {
                    printDocument.AddPage(pageVisual);
                }
            }

            printDocument.AddPagesComplete();
        }

        private UIElement BuildPicturePage(Int32 pageNumber, [NotNull] PrintLayout printLayout, Boolean useThumbnail, String pageTitle)
        {
            if (printLayout == null) throw new ArgumentNullException("printLayout");

            // Get the template visual element, using the callback provided by the selected print layout
            var template = printLayout.TemplateControlGetter();
            template.HorizontalAlignment = HorizontalAlignment.Stretch;
            template.VerticalAlignment = VerticalAlignment.Stretch;

            // Create and pass the view model to the visual element to populate the pictures
            var picturesToPrint = _pictureProvider();
            var templateViewModel = new PrintTemplateViewModel(pageNumber, printLayout.PicturesPerPage, picturesToPrint, useThumbnail, pageTitle);
            template.DataContext = templateViewModel;

            return template;
        }

        #endregion
    }
}