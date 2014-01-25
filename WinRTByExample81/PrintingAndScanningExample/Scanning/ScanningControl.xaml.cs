using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PrintingAndScanningExample
{
    public sealed partial class ScanningControl : UserControl
    {
        public ScanningControl()
        {
            InitializeComponent();
        }
    }

    public class DesignScanningControlViewModel : ScanningControlViewModel
    {
        public DesignScanningControlViewModel()
            : base(new ScannerHelper())
        {
            
        }
    }
}
