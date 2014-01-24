using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PrintingAndScanningExample
{
    public sealed partial class PictureModelLayout : UserControl
    {
        public PictureModelLayout()
        {
            InitializeComponent();
        }
    }

    public class DesignPictureModel : TemplatePictureModel
    {
        public DesignPictureModel() :
            base(new PictureModel
                 {
                     Caption = "Sample Picture",
                     Picture = new BitmapImage(new Uri("ms-appx:///Assets/Logo.png")),
                     Thumbnail = new BitmapImage(new Uri("ms-appx:///Assets/SmallLogo.png")),
                 }, false)
        {
        }
    }
}
