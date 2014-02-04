using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using Windows.UI.Xaml.Media.Imaging;

namespace PrintingAndScanningExample
{
    public sealed partial class ScanningControl : UserControl
    {
        public ScanningControl()
        {
            InitializeComponent();
        }

        private void HandlePreviewImageLayoutUpdated(object sender, object e)
        {
            HorizontalSlider.Width = PreviewImage.ActualWidth;
            VerticalSlider.Height = PreviewImage.ActualHeight;
        }
    }

    public class DesignScanningControlViewModel : ScanningControlViewModel
    {
        public DesignScanningControlViewModel()
            : base(new ScannerHelper())
        {
            PreviewImage = new BitmapImage(new Uri("ms-appx:///Assets/Logo.png"));
        }
    }
}
