using Windows.UI.Xaml.Controls;

namespace InputsExample
{
    public sealed partial class SquareControl : UserControl
    {
        private readonly InputEventHandler _inputEventHandler;
        public SquareControl()
        {
            InitializeComponent();
            _inputEventHandler = new InputEventHandler(InnerShape);
        }
    }
}
