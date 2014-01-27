using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.Graphics.Printing.OptionDetails;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Printing;

namespace PrintingAndScanningExample
{
    public class PrintHelper
    {
        #region Fields

        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        private readonly PrintDocument _printDocument;

        private readonly Func<IEnumerable<PictureModel>> _pictureProvider;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintHelper"/> class.
        /// </summary>
        public PrintHelper(Func<IEnumerable<PictureModel>> pictureProvider)
        {
            if (pictureProvider == null) throw new ArgumentNullException("pictureProvider");
            
            // Store the callback to use to obtain the list of pictures to print
            _pictureProvider = pictureProvider;

            // Set up the Print Document
            _printDocument = new PrintDocument();

            // This handler for the Paginate event is raised to request the count of preview pages
            _printDocument.Paginate += HandlePagination;

            // This requests/receives a specific print preview page
            _printDocument.GetPreviewPage += HandleGetPreviewPage;

            // This is called when the system is ready to start printing and provides the final pages
            _printDocument.AddPages += HandleAddPages;
        } 

        #endregion

        #region Interacting with the Print Manager

        /// <summary>
        /// Attaches to the printing process and configures the various event handlers.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">pictureCollection</exception>
        public void ConfigurePrinting()
        {
            // Connect to the PrintManager and so the app will be notified of requests to print
            var printManager = PrintManager.GetForCurrentView();
            printManager.PrintTaskRequested += HandlePrintTaskRequested;

            //// Important note: If this is handler is subscribed to more than once, an exception is thrown
            //try
            //{
            //    printManager.PrintTaskRequested += HandlePrintTaskRequested;
            //}
            //catch (InvalidOperationException e)
            //{
            //    Debug.WriteLine("Printing configuration error: {0}", e);
            //}
        }

        public void DetachPrinting()
        {
            // Detach from the PrintManager's PrintTaskRequested event
            var printManager = PrintManager.GetForCurrentView();
            printManager.PrintTaskRequested -= HandlePrintTaskRequested;
        }

        public async Task ShowPrintPreview()
        {
            await PrintManager.ShowPrintUIAsync();
        } 

        #endregion
        
        #region Print Task (Job) Configuration

        private void HandlePrintTaskRequested
            (PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            // Called when printing is first requested (Device Charm / Printing or programmatically)
            Debug.WriteLine("PrintTask Requested");

            // Make sure there actually are some pictures to be printed.  Returning without defining a task will
            // inform the user the app is not currently ready to print before even letting them choose a printer.
            var picturesToBePrinted = _pictureProvider();
            if (!picturesToBePrinted.Any()) return;

            // Create the task, which includes a name and a callback handler for what to do when a printer is actually selected
            // If a task is created in this way, there is an expectation that printing is currently possible.
            var printTask = args.Request.CreatePrintTask(
                "WinRT Printing Example", 
                HandlePrintTaskSourceRequested);

            // Subscribe to lifecycle events for the current task/job 
            // Note - Unsubscribe in "completed", which will always be called
            printTask.Previewing += HandlePrintTaskPreviewing;  // Called before pages are requested for preview
            printTask.Submitting += HandlePrintTaskSubmitting;  // Called when the user hits "print"
            printTask.Progressing += HandlePrintTaskProgressing;  // Called once for each page that is printed
            printTask.Completed += HandlePrintTaskCompleted;  // Called after all pages are printed

            // Define the list of print options to be shown for the current task/job
            // Indicate which standard options should be included/not included
            var printOptions = printTask.Options.DisplayedOptions;
            printOptions.Clear();
            printOptions.Add(StandardPrintTaskOptions.Copies);
            printOptions.Add(StandardPrintTaskOptions.Orientation);

            // Change the default orienation to Landscape
            printTask.Options.Orientation = PrintOrientation.Landscape;

            // Add/incorporate the app's custom print options into the list of print options
            ConfigureCustomOptions(printTask.Options);
            
            // If the work done to pull together the PrintTask is asynchronous, need to work with a deferral
            // var deferral = args.Request.GetDeferral();
            // Work that contains async operations goes here
            // deferral.Complete();
        }

        private void HandlePrintTaskSourceRequested
            (PrintTaskSourceRequestedArgs args)
        {
            // Called when a printer is selected from the printers list panel 
            Debug.WriteLine("PrintTask Source Requested");

            // Request a deferral to accommodate the async operation
            var deferral = args.GetDeferral();

            // Set the document source for the current print job.
            // This MUST happen on the UI thread.
            _syncContext.Post(x =>
            {
                args.SetSource(_printDocument.DocumentSource);

                // Complete the deferral to indicate completion
                deferral.Complete();
            }, null);
        }

        #endregion

        #region Custom Print Options

        private const String LayoutOptionId = "PageLayout";
        private const String PreviewTypeOptionId = "PreviewType";
        private const String PageTitleOptionId = "PageTitle";
        private const PrintLayoutId DefaultPrintLayoutId = PrintLayoutId.LayoutOneByOne;
        private const PreviewTypeOption DefaultPreviewTypeOption = PreviewTypeOption.Thumbnails;
        private const String DefaultPageTitle = "WinRT by Example";

        private enum PreviewTypeOption
        {
            Thumbnails,
            FullRes
        }

        private void ConfigureCustomOptions(PrintTaskOptions printTaskOptions)
        {
            if (printTaskOptions == null) 
                throw new ArgumentNullException("printTaskOptions");
            
            var detailedOptions = 
                PrintTaskOptionDetails.GetFromPrintTaskOptions(printTaskOptions);
            
            // Create the list of options for choosing a layout
            var selectedLayoutOption = detailedOptions.CreateItemListOption(
                LayoutOptionId, "Layout");
            selectedLayoutOption.AddItem(
                PrintLayoutId.LayoutOneByOne.ToString(), 
                PrintLayout.Layouts[PrintLayoutId.LayoutOneByOne].LayoutName);

            selectedLayoutOption.AddItem(
                PrintLayoutId.LayoutTwoByTwo.ToString(), 
                PrintLayout.Layouts[PrintLayoutId.LayoutTwoByTwo].LayoutName);

            selectedLayoutOption.TrySetValue(DefaultPrintLayoutId.ToString());
            detailedOptions.DisplayedOptions.Add(LayoutOptionId);

            // Create the list of options for choosing whether the preview 
            // should use thumbnails or full-res images
            var previewTypeOption = detailedOptions.CreateItemListOption(
                PreviewTypeOptionId, "Preview Type");
            previewTypeOption.AddItem(
                PreviewTypeOption.Thumbnails.ToString(), 
                "Thumbnails");
            previewTypeOption.AddItem(
                PreviewTypeOption.FullRes.ToString(), 
                "Full Res");

            previewTypeOption.TrySetValue(DefaultPreviewTypeOption.ToString());
            detailedOptions.DisplayedOptions.Add(PreviewTypeOptionId);

            // Create the option that allows users to provide a page title 
            // for the printout
            var pageTitleOption = detailedOptions.CreateTextOption(
                PageTitleOptionId, "Page Title");
            pageTitleOption.TrySetValue(DefaultPageTitle);
            detailedOptions.DisplayedOptions.Add(PageTitleOptionId);

            detailedOptions.OptionChanged += HandleCustomOptionsOptionChanged;

        }

        private void HandleCustomOptionsOptionChanged
            (PrintTaskOptionDetails sender, PrintTaskOptionChangedEventArgs args)
        {
            // Called in response to a setting being changed.  
            // Determine if it was a custom setting and react accordingly
            var optionId = args.OptionId as String;

            if (LayoutOptionId.Equals(optionId) 
                || PreviewTypeOptionId.Equals(optionId) 
                || PageTitleOptionId.Equals(optionId))
            {
                // Invalidate the preview content to force it to refresh.
                // This has to happen on the UI thread.
                _syncContext.Post(x => _printDocument.InvalidatePreview(), null);
            }
        }

        private PrintLayout GetSelectedPrintLayout(PrintTaskOptions printTaskOptions)
        {
            if (printTaskOptions == null) throw new ArgumentNullException("printTaskOptions");

            var detailedOptions = 
                PrintTaskOptionDetails.GetFromPrintTaskOptions(printTaskOptions);
            var result = PrintLayout.Layouts[DefaultPrintLayoutId];
            IPrintOptionDetails option;
            if (detailedOptions.Options.TryGetValue(LayoutOptionId, out option))
            {
                var selectedValueText = option.Value as String;
                if (!String.IsNullOrWhiteSpace(selectedValueText))
                {
                    var selectedLayout = (PrintLayoutId)Enum.Parse(typeof(PrintLayoutId), selectedValueText);
                    result = PrintLayout.Layouts[selectedLayout];
                }
            }

            return result;
        }

        private PreviewTypeOption GetSelectedPreviewType(PrintTaskOptions printTaskOptions)
        {
            if (printTaskOptions == null) throw new ArgumentNullException("printTaskOptions");

            var detailedOptions = 
                PrintTaskOptionDetails.GetFromPrintTaskOptions(printTaskOptions);
            var result = DefaultPreviewTypeOption;
            IPrintOptionDetails option;
            if (detailedOptions.Options.TryGetValue(PreviewTypeOptionId, out option))
            {
                var selectedValueText = option.Value as String;
                if (!String.IsNullOrWhiteSpace(selectedValueText))
                {
                    result = (PreviewTypeOption)Enum.Parse(typeof(PreviewTypeOption), selectedValueText);
                }
            }
            return result;
        }

        public String GetPageTitle(PrintTaskOptions printTaskOptions)
        {
            if (printTaskOptions == null) throw new ArgumentNullException("printTaskOptions");

            var detailedOptions = 
                PrintTaskOptionDetails.GetFromPrintTaskOptions(printTaskOptions);
            var result = DefaultPageTitle;
            IPrintOptionDetails option;
            if (detailedOptions.Options.TryGetValue(PageTitleOptionId, out option))
            {
                result = option.Value as String;
            }
            return result;
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
            _currentPreviewOptions.PageTitle = GetPageTitle(printOptions);
            _currentPreviewOptions.SelectedLayout = GetSelectedPrintLayout(printOptions);
            _currentPreviewOptions.PreviewType = GetSelectedPreviewType(printOptions);

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
            var pageTitle = GetPageTitle(args.PrintTaskOptions);
            var picturesToPrint = _pictureProvider();

            // Determine the value of the layout template setting
            var selectedLayout = GetSelectedPrintLayout(options);

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

        private UIElement BuildPicturePage(Int32 pageNumber, PrintLayout printLayout, Boolean useThumbnail, String pageTitle)
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