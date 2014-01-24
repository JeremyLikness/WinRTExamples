using System;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace PrintingAndScanningExample
{
    public class PrintLayout
    {
        public static Dictionary<PrintLayoutId, PrintLayout> PrintLayouts = new Dictionary<PrintLayoutId, PrintLayout>
        {
            {PrintLayoutId.LayoutOneByOne, new PrintLayout
                                        {
                                            LayoutName = "Single Image", 
                                            PicturesPerPage = 1,
                                            TemplateControlGetter = () => new Template1x1()
                                        }},
            {PrintLayoutId.LayoutTwoByTwo, new PrintLayout
                                        {
                                            LayoutName = "Two by two", 
                                            PicturesPerPage = 4,
                                            TemplateControlGetter = () => new Template2x2()
                                        }},
        };



        public String LayoutName { get; private set; }

        public Int32 PicturesPerPage { get; private set; }

        public Func<FrameworkElement> TemplateControlGetter { get; private set; }
    }

    public enum PrintLayoutId
    {
        LayoutOneByOne,
        LayoutTwoByTwo,
    }
}