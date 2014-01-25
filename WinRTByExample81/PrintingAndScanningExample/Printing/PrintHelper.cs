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
        private readonly PrintManager _printManager = PrintManager.GetForCurrentView();
        private readonly PrintDocument _printDocument = new PrintDocument();

        private readonly CustomPrintLayoutOptionHelper _customPrintOptionsHelper = new CustomPrintLayoutOptionHelper();

        private IList<PictureModel> _pictureCollection;
        
        private readonly List<UIElement> _previewPages = new List<UIElement>();

        #endregion

        #region Public Interface

        // TODO - re-order events and handlers to better show order of occurrence
        public void ConfigurePrinting([NotNull] IList<PictureModel> pictureCollection)
        {
            if (pictureCollection == null) throw new ArgumentNullException("pictureCollection");
            _pictureCollection = pictureCollection;

            // This handler for the Paginate event is raised to request the count of preview pages
            _printDocument.Paginate += HandlePagination;

            // This requests/receives a specific print preview page
            _printDocument.GetPreviewPage += HandleGetPreviewPage;

            // This is called when the system is ready to start printing and provides the final pages
            _printDocument.AddPages += HandleAddPages;

            // If this handler is declared, printers are shown when the Device Charm is
            // invoked.  Put another way, when there is a handler, there is an expectation that
            // printing is currently possible.
            // Likewise, if this is handler is subscribed to twice, an exception is thrown
            _printManager.PrintTaskRequested += HandlePrintTaskRequested;

            // Event raised when a change is made to the the custom print option 
            _customPrintOptionsHelper.CustomPrintOptionChanged += HandleCustomPrintOptionHelperTemplateOptionChanged;
        }

        public void DetachPrinting()
        {
            // Detach from the Print Task handler to remove the expectation that printing is available
            _printManager.PrintTaskRequested -= HandlePrintTaskRequested;
        }

        public async Task ShowPrintPreview()
        {
            await PrintManager.ShowPrintUIAsync();
        } 

        #endregion

        private void HandlePagination(Object sender, PaginateEventArgs args)
        {
            
            var printOptions = args.PrintTaskOptions;

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

            var pageTitle = _customPrintOptionsHelper.GetPageTitle(printOptions);
            var selectedLayout = _customPrintOptionsHelper.GetSelectedPrintLayout(printOptions);
            
            // Calculate the total page count based on number of pictures per page and total number of selected pictures
            var totalPageCount = (Int32)Math.Ceiling(_pictureCollection.Count() / (Double)selectedLayout.PicturesPerPage);

            // Build up a collection of indexed print preview pages for retrieval in the GetPreviewPage event
            _previewPages.Clear();
            var previewType = _customPrintOptionsHelper.GetSelectedPreviewType(printOptions);
            for (Int32 i = 1; i <= totalPageCount; i++)
            {
                var pageVisual = GetPicturePage(i, selectedLayout, previewType == PreviewTypeOption.Thumbnails, pageTitle);
                _previewPages.Add(pageVisual);
            }

            // TODO - Look into / discuss distinction between Intermediate and final count indication
            _printDocument.SetPreviewPageCount(totalPageCount, PreviewPageCountType.Intermediate);// PreviewPageCountType.Final);

            // TODO - Raise Notification that pages have been defined
            Debug.WriteLine("Pagination has completed.");
        }

        private void HandleGetPreviewPage(Object sender, GetPreviewPageEventArgs args)
        {
            // Provide the print preview content for the requested page
            var currentPage = args.PageNumber;
            _printDocument.SetPreviewPage(currentPage, _previewPages[currentPage-1]);
        }


        private void HandleAddPages(Object sender, AddPagesEventArgs args)
        {
            // Build the collection of pages to display and pass them to the print document

            // Get the page title selection
            var pageTitle = _customPrintOptionsHelper.GetPageTitle(args.PrintTaskOptions);

            // Determine the value of the layout template setting
            var selectedLayout = _customPrintOptionsHelper.GetSelectedPrintLayout(args.PrintTaskOptions);

            // args.PrintTaskOptions.GetPageDescription()
            var pageCount = Math.Ceiling(_pictureCollection.Count() / (Double)selectedLayout.PicturesPerPage);
            for (var i = 1; i <= pageCount; i++)
            {
                var pageVisual = GetPicturePage(i, selectedLayout, false, pageTitle);
                {
                    _printDocument.AddPage(pageVisual);   
                }
            }

            _printDocument.AddPagesComplete();
        }

        private void HandlePrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            // If the work done to pull together the PrintTask is asynchronous, need to work a deferral
            //var deferral = args.Request.GetDeferral();
            var printTask = args.Request.CreatePrintTask("Windows Runtime By Example Print Example", HandlePrintSourceRequested);

            var customOptions = _customPrintOptionsHelper.ConfigureCustomOptions(printTask.Options);

            // Define the list of print options to be shown (including the custom option just defined)
            var previewDisplayedOptions = printTask.Options.DisplayedOptions;
            previewDisplayedOptions.Clear();
            previewDisplayedOptions.Add(StandardPrintTaskOptions.Copies);
            previewDisplayedOptions.Add(StandardPrintTaskOptions.Orientation);
            foreach (var customOption in customOptions)
            {
                previewDisplayedOptions.Add(customOption.OptionId);    
            }

            // Called before pages are requested for preview
            printTask.Previewing += HandlePrintTaskPreviewing;

            // Called when the user hits "print"
            printTask.Submitting += HandlePrintTaskSubmitting;
            
            // Called once for each page that is printed
            printTask.Progressing += HandlePrintTaskProgressing;

            // Called after all pages are printed
            printTask.Completed += HandlePrintTaskCompleted;

            //deferral.Complete();
        }

        private void HandleCustomPrintOptionHelperTemplateOptionChanged(Object sender, EventArgs eventArgs)
        {
            // Called to inform that the custom template setting has changed
            // Refresh all of teh content by invalidating the current preview
            // This has to happen on the UI thread, though the call is originating elsewhere.
            _syncContext.Post(x => _printDocument.InvalidatePreview(), null);
        }

        private void HandlePrintSourceRequested(PrintTaskSourceRequestedArgs args)
        {
            // Called when a printer device is selected

            // args.Deadline
            var deferral = args.GetDeferral();

            // Set the document source for the current print request.
            // HAS TO HAPPEN ON THE UI THREAD!
            _syncContext.Post(x => 
            {
                args.SetSource(_printDocument.DocumentSource);
                deferral.Complete();
            }, null);
        }

        #region Job Status Events
        
        private void HandlePrintTaskPreviewing(PrintTask sender, Object args)
        {
            // TODO - Note that the event may becoming on wrong thread...need to marshall?
            // Notify the caller that the print job is being previewed
            Debug.WriteLine("Print Task {0} is Previewing", sender.Properties.Title);
        }

        private void HandlePrintTaskSubmitting(PrintTask sender, Object args)
        {
            // TODO - Note that the event may becoming on wrong thread...need to marshall?
            // Notify the caller that the print job is being submitted
            Debug.WriteLine("Print Task {0} is Being Submitted", sender.Properties.Title);
        }

        private void HandlePrintTaskProgressing(PrintTask sender, PrintTaskProgressingEventArgs args)
        {
            // TODO - Note that the event may becoming on wrong thread...need to marshall?
            // Notify the caller that the print job is making progress
            Debug.WriteLine("Print Task {0} is Progressing - out of {1}", sender.Properties.Title, args.DocumentPageCount);
        }
        
        private void HandlePrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
        {
            // TODO - Note that the event may becoming on wrong thread...need to marshall?
            // Notify the calling app of the print completion reason
            //args.Completion == PrintTaskCompletion.Submitted PrintTaskCompletion.Failed PrintTaskCompletion.Canceled  PrintTaskCompletion.Abandoned
            Debug.WriteLine("Print Task {0} has completed with a status of {1}.", sender.Properties.Title, args.Completion);
        }

        #endregion

        private UIElement GetPicturePage(Int32 pageNumber, [NotNull] PrintLayout printLayout, Boolean useThumbnail, String pageTitle)
        {
            if (printLayout == null) throw new ArgumentNullException("printLayout");
            
            // Get the template visual element, using the callback provided by the selected print layout
            var template = printLayout.TemplateControlGetter();
            template.HorizontalAlignment = HorizontalAlignment.Stretch;
            template.VerticalAlignment = VerticalAlignment.Stretch;
            
            // Create and pass the view model to the visual element to populate the pictures
            // TODO - account for thumbnail in picture selection from view model
            var templateViewModel = new PrintTemplateViewModel(pageNumber, printLayout.PicturesPerPage, _pictureCollection, useThumbnail, pageTitle);
            template.DataContext = templateViewModel;

            return template;
        }
    }
}