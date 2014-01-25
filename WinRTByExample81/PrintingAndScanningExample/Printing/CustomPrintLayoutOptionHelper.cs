using System;
using System.Collections.Generic;
using Windows.Graphics.Printing;
using Windows.Graphics.Printing.OptionDetails;
using PrintingAndScanningExample.Annotations;

namespace PrintingAndScanningExample
{
    public class CustomPrintLayoutOptionHelper
    {
        private const String LayoutOptionId = "PageLayout";
        private const String PreviewTypeOptionId = "PreviewType";
        private const String PageTitleOptionId = "PageTitle";

        public IEnumerable<IPrintOptionDetails> ConfigureCustomOptions([NotNull] PrintTaskOptions printTaskOptions)
        {
            if (printTaskOptions == null) throw new ArgumentNullException("printTaskOptions");

            var printDetailedOptions = PrintTaskOptionDetails.GetFromPrintTaskOptions(printTaskOptions);
            printDetailedOptions.OptionChanged += HandleCustomOptionsOptionChanged;

            // Create the list of options for choosing a layout
            var selectedLayoutOption = printDetailedOptions.CreateItemListOption(LayoutOptionId, "Layout");
            selectedLayoutOption.AddItem(PrintLayoutId.LayoutOneByOne.ToString(), PrintLayout.PrintLayouts[PrintLayoutId.LayoutOneByOne].LayoutName);
            selectedLayoutOption.AddItem(PrintLayoutId.LayoutTwoByTwo.ToString(), PrintLayout.PrintLayouts[PrintLayoutId.LayoutTwoByTwo].LayoutName);

            var previewTypeOption = printDetailedOptions.CreateItemListOption(PreviewTypeOptionId, "Preview Type");
            previewTypeOption.AddItem(PreviewTypeOption.Thumbnails.ToString(), "Thumbnails");
            previewTypeOption.AddItem(PreviewTypeOption.FullRes.ToString(), "Full Res");

            var pageTitleOption = printDetailedOptions.CreateTextOption(PageTitleOptionId, "Page Title");
            pageTitleOption.TrySetValue("Windows Runtime by Example");

            return new IPrintOptionDetails[] { selectedLayoutOption, previewTypeOption, pageTitleOption };
        }

        public PrintLayout GetSelectedPrintLayout([NotNull] PrintTaskOptions printTaskOptions)
        {
            if (printTaskOptions == null) throw new ArgumentNullException("printTaskOptions");

            PrintLayout result = null;

            var printDetailedOptions = PrintTaskOptionDetails.GetFromPrintTaskOptions(printTaskOptions);
            var option = printDetailedOptions.Options[LayoutOptionId];
            var selectedValueText = option.Value as String;
            if (!String.IsNullOrWhiteSpace(selectedValueText))
            {
                var selectedLayout = (PrintLayoutId)Enum.Parse(typeof(PrintLayoutId), selectedValueText);
                result = PrintLayout.PrintLayouts[selectedLayout];
            }
            return result;
        }

        public PreviewTypeOption GetSelectedPreviewType([NotNull] PrintTaskOptions printTaskOptions)
        {
            if (printTaskOptions == null) throw new ArgumentNullException("printTaskOptions");

            var result = PreviewTypeOption.Thumbnails;

            var printDetailedOptions = PrintTaskOptionDetails.GetFromPrintTaskOptions(printTaskOptions);
            var option = printDetailedOptions.Options[PreviewTypeOptionId];
            var selectedValueText = option.Value as String;
            if (!String.IsNullOrWhiteSpace(selectedValueText))
            {
                result = (PreviewTypeOption)Enum.Parse(typeof(PreviewTypeOption), selectedValueText);
            }
            return result;
        }

        public String GetPageTitle([NotNull] PrintTaskOptions printTaskOptions)
        {
            if (printTaskOptions == null) throw new ArgumentNullException("printTaskOptions");

            var printDetailedOptions = PrintTaskOptionDetails.GetFromPrintTaskOptions(printTaskOptions);
            var option = printDetailedOptions.Options[PageTitleOptionId];
            var result = option.Value as String;
            return result;
        }

        private void HandleCustomOptionsOptionChanged(PrintTaskOptionDetails sender, PrintTaskOptionChangedEventArgs args)
        {
            var optionId = args.OptionId as String;
            if (String.IsNullOrEmpty(optionId)) return;

            if (LayoutOptionId.Equals(optionId) || PreviewTypeOptionId.Equals(optionId) || PageTitleOptionId.Equals(optionId))
            {
                OnCustomPrintOptionChanged();
            }
        }

        public event EventHandler CustomPrintOptionChanged;

        protected virtual void OnCustomPrintOptionChanged()
        {
            EventHandler handler = CustomPrintOptionChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }

    public enum PreviewTypeOption
    {
        Thumbnails,
        FullRes
    }
}