using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace InputsExample
{
    public sealed partial class BallControl : UserControl
    {
        private readonly InputEventHandler _inputEventHandler;
        public BallControl()
        {
            InitializeComponent();
            _inputEventHandler = new InputEventHandler(InnerShape);
        }
    }
}
