using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PrintingAndScanningExample
{
    public sealed partial class Template1x1 : UserControl
    {
        public Template1x1()
        {
            InitializeComponent();
        }
    }

    public class DesignPrintTemplate1x1ViewModel : PrintTemplateViewModel
    {
        private static readonly List<PictureModel> SamplePictures = new List<PictureModel>
            {
                new PictureModel
                {
                    Caption = "Sample One",
                    Picture = new BitmapImage(new Uri("ms-appx:///Assets/Logo.png")),
                    Thumbnail = new BitmapImage(new Uri("ms-appx:///Assets/SmallLogo.png"))
                },
            };

        public DesignPrintTemplate1x1ViewModel()
            : base(0, 1, SamplePictures, false, "1 x 1 Design Template")
        {
        }
    }
}
