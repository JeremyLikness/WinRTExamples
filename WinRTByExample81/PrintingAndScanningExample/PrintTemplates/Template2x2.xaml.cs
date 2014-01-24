using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PrintingAndScanningExample
{
    public sealed partial class Template2x2 : UserControl
    {
        public Template2x2()
        {
            InitializeComponent();
        }
    }

    public class DesignPrintTemplate2x2ViewModel : DesignPrintTemplateViewModelBase
    {
        public DesignPrintTemplate2x2ViewModel()
            : base(0, 4, SamplePictures, false, "2 x 2 Design Template")
        {
        }
    }
}
