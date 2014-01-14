using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using InputsExample.Common;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace InputsExample
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class InputsPage : Page
    {
        #region Fields

        private readonly NavigationHelper _navigationHelper;
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();

        private readonly DispatcherTimer _animationTimer = new DispatcherTimer();
        private readonly ObservableCollection<ShapeModel> _shapes = new ObservableCollection<ShapeModel>();
        private readonly InputSettings _inputSettings = new InputSettings(); 

        #endregion

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }


        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="InputsPage"/> class.
        /// </summary>
        public InputsPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += HandleNavigationHelperLoadState;
            _navigationHelper.SaveState += HandleNavigationHelperSaveState;

            SizeChanged += OnSizeChanged;

            // Subscribe to changes in the input settings
            _inputSettings.PropertyChanged += (sender, args) => UpdateStateFromInputSettings();

            // Make sure the page has input focus so that keyboard events are available immediately
            Loaded += (sender, args) => Focus(FocusState.Programmatic);

            // Initialize the animation timer
            _animationTimer.Interval = TimeSpan.FromMilliseconds(30);
            _animationTimer.Tick += HandleAnimationTimer;
        } 

        #endregion

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void HandleNavigationHelperLoadState(Object sender, LoadStateEventArgs e)
        {
            DefaultViewModel["Shapes"] = _shapes;
            DefaultViewModel["InputSettings"] = _inputSettings;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void HandleNavigationHelperSaveState(Object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void OnSizeChanged(Object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            // Compensate for screen size change - make sure everything is put back within the new screen dimensions
            StopAnimationTimer();

            foreach (var shape in _shapes)
            {
                shape.UpdateExtents(new Point(0, 0), new Point(ShapePanel.ActualWidth, ShapePanel.ActualHeight));
            }

            UpdateAnimationTimerState();
        }

        private void CreateShape(ShapeModel.ShapeType shapeType)
        {
            var minPosPoint = new Point(0, 0);
            var maxPosPoint = new Point(ShapePanel.ActualWidth, ShapePanel.ActualHeight);
            var initialPosition = new Point((maxPosPoint.X - minPosPoint.X)/2, (maxPosPoint.Y - minPosPoint.Y)/2);

            ShapeModel shape;
            switch (shapeType)
            {
                case ShapeModel.ShapeType.Ball:
                    shape = new BallShapeModel();
                    break;
                case ShapeModel.ShapeType.Square:
                    shape = new SquareShapeModel();
                    break;
                default:
                    throw new InvalidOperationException("Unexpected Shape Type");
            }

            shape.Rate = 8;
            shape.MinPosPoint = minPosPoint;
            shape.MaxPosPoint = maxPosPoint;
            shape.Position = initialPosition;

            _shapes.Add(shape);

            UpdateStateFromInputSettings();
        }

        private void StopAnimationTimer()
        {
            _animationTimer.Stop();
        }

        private void UpdateAnimationTimerState()
        {
            if (_inputSettings.IsAnimationOn) _animationTimer.Start();
            else _animationTimer.Stop();
        }

        private void HandleAnimationTimer(Object sender, Object o)
        {
            StopAnimationTimer();
            foreach (var shape in _shapes.Where(x => !x.IsHot))
            {
                shape.UpdateShapePosition();
            }
            UpdateAnimationTimerState();
        }

        private void UpdateStateFromInputSettings()
        {
            // Turn the timer on/off based on settings
            UpdateAnimationTimerState();

            // Update each shape based on the settings values
            foreach (var shape in _shapes)
            {
                shape.UpdateInputSettings(_inputSettings);
            }
        }

        private void HandleMouseDetailsClick(Object sender, RoutedEventArgs e)
        {
            var capabilities = new MouseCapabilities();
            String message;
            if (capabilities.MousePresent == 1)
            {
                var rawMessage =
                    "There is a mouse present.  " + 
                    "The connected mice have a max of {0} buttons.  " + 
                    "There {1} a vertical wheel present.  " + 
                    "There {2} a horizontal wheel present. "  + 
                    "Mouse buttons {3} been swapped.";

                message = String.Format(rawMessage
                    , capabilities.NumberOfButtons
                    , capabilities.VerticalWheelPresent == 1 ? "is" : "is not"
                    , capabilities.HorizontalWheelPresent == 1 ? "is" : "is not"
                    , capabilities.SwapButtons == 1 ? "have" : "have not"
                    );
            }
            else
            {
                message = "There are no mice present.";
            }
            ShowMessage(message, "Mouse Properties");
        }

        private void HandleTouchDetailsClick(Object sender, RoutedEventArgs e)
        {
            var capabilities = new TouchCapabilities();
            String message;
            if (capabilities.TouchPresent == 1)
            {
                var rawMessage = 
                    "Touch support is available.  " + 
                    "Up to {0} touch points are supported.";

                message = String.Format(rawMessage, capabilities.Contacts);
            }
            else
            {
                message = "Touch support is not available.";
            }
            ShowMessage(message, "Touch Properties");
        }

        private void HandleKeyboardDetailsClick(Object sender, RoutedEventArgs e)
        {
            var keyboardCapabilities = new KeyboardCapabilities();
            var message = keyboardCapabilities.KeyboardPresent == 1
                ? "There is a keyboard present."
                : "There is no keyboard present.";

            ShowMessage(message, "Keyboard Properties");
        }

        private async void ShowMessage(String content, String title)
        {
            var messageDialog = new MessageDialog(content, title);
            await messageDialog.ShowAsync();
        }

        private void HandleBackgroundTapped(Object sender, TappedRoutedEventArgs e)
        {
            // Adding this extra tap handler so that clicking on the background causes something to get focus so that keybaord actions can be raised/caught.
            Focus(FocusState.Programmatic);
        }

        protected override void OnKeyDown(KeyRoutedEventArgs args)
        {
            // Check for shift, control, alt (AKA VirtualKey.Menu)
            var currentWindow = CoreWindow.GetForCurrentThread();
            var ctrlState = currentWindow.GetKeyState(VirtualKey.Control);
            var shftState = currentWindow.GetKeyState(VirtualKey.Shift);
            var altState = currentWindow.GetKeyState(VirtualKey.Menu);
            var isControlKeyPressed =
                (ctrlState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
            var isShiftKeyPressed =
                (shftState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
            var isAltKeyPressed =
                (altState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;

            System.Diagnostics.Debug.WriteLine("KeyDown: {0} WasDown = {1}, Ctrl={2}, Shift={3}, Alt={4}", 
                args.Key,
                args.KeyStatus.WasKeyDown, 
                isControlKeyPressed,
                isShiftKeyPressed,
                isAltKeyPressed);
        }

        protected override void OnKeyUp(KeyRoutedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("KeyUp: {0} - {1}, {2}", args.Key, args.KeyStatus.WasKeyDown, args.KeyStatus.IsKeyReleased);
            if (args.Key == VirtualKey.B) CreateShape(ShapeModel.ShapeType.Ball);
            if (args.Key == VirtualKey.S) CreateShape(ShapeModel.ShapeType.Square);
        }

        private void HandlePointerDetailsPressed(Object sender, PointerRoutedEventArgs e)
        {
            // Show a flyout that displays the PointerPoint details for the current pointer press
            var senderElement = (FrameworkElement)sender;
            var pointerPoint = e.GetCurrentPoint(senderElement);
            var pointerPointValues = pointerPoint.GetPointerPointDump();
            DefaultViewModel["PointerPointValues"] = pointerPointValues;
            FlyoutBase.ShowAttachedFlyout(senderElement);
        }

        private void HandleAddBallClick(Object sender, RoutedEventArgs e)
        {
            CreateShape(ShapeModel.ShapeType.Ball);
        }

        private void HandleAddSquareClick(Object sender, RoutedEventArgs e)
        {
            CreateShape(ShapeModel.ShapeType.Square);
        }
    }
}
