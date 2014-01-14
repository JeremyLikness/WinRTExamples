using System;
using System.Linq;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace InputsExample
{
    // TODO - Relay events to log/display area

    public class InputEventHandler
    {
        #region Fields

        private ShapeModel _shapeModel;
        private readonly FrameworkElement _eventSourceElement;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="InputEventHandler"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <exception cref="System.ArgumentNullException">element</exception>
        public InputEventHandler(FrameworkElement element)
        {
            if (element == null) throw new ArgumentNullException("element");

            _eventSourceElement = element;

            // Get the model that gets data-bound to the shape element that this handler is working for.
            element.DataContextChanged += (sender, args) =>
                                          {
                                              _shapeModel = args.NewValue as ShapeModel;
                                          };

            // Subscribe to the various Input Device events
            // Pointer
            element.PointerEntered += HandlePointerEntered;
            element.PointerExited += HandlePointerExited;
            element.PointerPressed += HandlePointerPressed;
            element.PointerMoved += HandlePointerMoved;
            element.PointerReleased += HandlePointerReleased;
            element.PointerCanceled += HandlePointerCanceled;
            element.PointerCaptureLost += HandlePointerCaptureLost;

            // Manipulation
            element.ManipulationStarting += HandleManipulationStarting;
            element.ManipulationStarted += HandleManipulationStarted;
            element.ManipulationDelta += HandleManipulationDelta;
            element.ManipulationInertiaStarting += HandleManipulationInertiaStarting; 
            element.ManipulationCompleted += HandleManipulationCompleted;

            // Gesture
            element.Tapped += HandleTapped;
            element.DoubleTapped += HandleDoubleTapped;
            element.RightTapped += HandleRightTapped;
            element.Holding += HandleHolding;
        }

        #endregion

        #region Pointer

        private void HandlePointerEntered(Object sender, PointerRoutedEventArgs args)
        {
            // Check to see if this kind of device is being ignored
            if (!IsValidDevice(args.Pointer.PointerDeviceType)) return;

            System.Diagnostics.Debug.WriteLine("PointerEntered - {0}, {1}", args.Pointer.PointerId, args.Pointer.PointerDeviceType);
            _shapeModel.IsHot = true;
        }

        private void HandlePointerExited(Object sender, PointerRoutedEventArgs args)
        {
            // Check to see if this kind of device is being ignored
            if (!IsValidDevice(args.Pointer.PointerDeviceType)) return;

            System.Diagnostics.Debug.WriteLine("PointerExited - {0}, {1}", args.Pointer.PointerId, args.Pointer.PointerDeviceType);
            _shapeModel.IsHot = false;
        }

        private void HandlePointerPressed(Object sender, PointerRoutedEventArgs args)
        {
            // Check to see if this kind of device is being ignored
            if (!IsValidDevice(args.Pointer.PointerDeviceType)) return;

            System.Diagnostics.Debug.WriteLine("PointerPressed - {0}, {1}", args.Pointer.PointerId, args.Pointer.PointerDeviceType);
            // Capturing the pointer to follow it as it strays outside the element
            _eventSourceElement.CapturePointer(args.Pointer);
        }

        private void HandlePointerMoved(Object sender, PointerRoutedEventArgs args)
        {
            // Check to see if this kind of device is being ignored
            if (!IsValidDevice(args.Pointer.PointerDeviceType)) return;
            System.Diagnostics.Debug.WriteLine("PointerMoved - {0}, {1}", args.Pointer.PointerId, args.Pointer.PointerDeviceType);
        }

        private void HandlePointerReleased(Object sender, PointerRoutedEventArgs args)
        {
            // Check to see if this kind of device is being ignored
            if (!IsValidDevice(args.Pointer.PointerDeviceType)) return;

            System.Diagnostics.Debug.WriteLine("PointerReleased - {0}, {1}", args.Pointer.PointerId, args.Pointer.PointerDeviceType);

            // No need to explicitly release the captured pointer - the pointer release automatically handles it
            //_eventSourceElement.ReleasePointerCapture(args.Pointer);
        }

        private void HandlePointerCanceled(Object sender, PointerRoutedEventArgs args)
        {
            // Check to see if this kind of device is being ignored
            if (!IsValidDevice(args.Pointer.PointerDeviceType)) return;

            System.Diagnostics.Debug.WriteLine("PointerCanceled - {0}, {1}", args.Pointer.PointerId, args.Pointer.PointerDeviceType);

            // No need to release the captured pointer - the pointer cancel automatically handles it (though CaptureLost might not be raised)
            // _eventSourceElement.ReleasePointerCapture(args.Pointer);
        }

        private void HandlePointerCaptureLost(Object sender, PointerRoutedEventArgs args)
        {
            // No need to release the captured pointer - this event indicates that it already happened.
            System.Diagnostics.Debug.WriteLine("PointerCaptureLost - {0}, {1}", args.Pointer.PointerId, args.Pointer.PointerDeviceType);
        }


        #endregion

        #region Manipulations

        private void HandleManipulationStarting(Object sender, ManipulationStartingRoutedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("ManipulationStarting");

            // Determine the current manipulation mode
            var manipulationMode = _shapeModel.ManipulationMode;

            // Set the available modes for the manipulation that is about to start
            args.Mode = manipulationMode;
        }

        private void HandleManipulationStarted(Object sender, ManipulationStartedRoutedEventArgs args)
        {
            // Check to see if this kind of device is being ignored
            if (!IsValidDevice(args.PointerDeviceType)) return;
            System.Diagnostics.Debug.WriteLine("ManipulationStarted");
        }

        private void HandleManipulationDelta(Object sender, ManipulationDeltaRoutedEventArgs args)
        {
            // Check to see if this kind of device is being ignored
            if (!IsValidDevice(args.PointerDeviceType)) return;
            System.Diagnostics.Debug.WriteLine("ManipulationDelta");

            // Update the shape display based on the delta values
            var delta = args.Delta;
            _shapeModel.MoveShape(delta.Translation.X, delta.Translation.Y);
            _shapeModel.ResizeShape(delta.Scale);
            _shapeModel.RotateShape(delta.Rotation);

            // args.PointerDeviceType
            // args.Position
            // args.Container // UI Element that contains the manipulation
            // args.IsInertial // Is this occurring as part of inertia?

            // Delta is most recent changes
            // args.Delta.Translation
            // args.Delta.Expansion // Pixels
            // args.Delta.Scale // Percentage
            // args.Delta.Rotation

            // Cumulative is changes since manipulation was started
            // args.Cumulative. // This is a delta (same props as above), all since manipulation was started

            // args.Velocities.Angular // Rotational velocity, degrees per ms
            // args.Velocities.Expansion // Scaling change in Pixels/ms
            // args.Velocities.Linear // Line velocity in Pixels/ms
        }

        private void HandleManipulationInertiaStarting(Object sender, ManipulationInertiaStartingRoutedEventArgs args)
        {
            // Check to see if this kind of device is being ignored
            if (!IsValidDevice(args.PointerDeviceType)) return;

            // Changes here can include adjusting the deceleration value or setting a target end value
            // args.RotationBehavior.DesiredDeceleration = .000001;
            System.Diagnostics.Debug.WriteLine("ManipulationInertia");
        }

        private void HandleManipulationCompleted(Object sender, ManipulationCompletedRoutedEventArgs args)
        {
            // Check to see if this kind of device is being ignored
            if (!IsValidDevice(args.PointerDeviceType)) return;
            System.Diagnostics.Debug.WriteLine("ManipulationCompleted");
        }

        #endregion

        #region Gestures

        private void HandleTapped(Object sender, TappedRoutedEventArgs args)
        {
            // Check to see if this kind of device is being ignored
            if (!IsValidDevice(args.PointerDeviceType)) return;

            // Examine the current position
            var position = args.GetPosition(_eventSourceElement);
            System.Diagnostics.Debug.WriteLine("Tapped at X={0}, Y={1}", position.X, position.Y);

            // Alter the shape based on the gesture performed
            _shapeModel.SetRandomColor();
        }

        private void HandleDoubleTapped(Object sender, DoubleTappedRoutedEventArgs args)
        {
            // Check to see if this kind of device is being ignored
            if (!IsValidDevice(args.PointerDeviceType)) return;

            // Examine the current position
            var position = args.GetPosition(_eventSourceElement);
            System.Diagnostics.Debug.WriteLine("DoubleTapped at X={0}, Y={1}", position.X, position.Y);

            // Alter the shape based on the gesture performed
            _shapeModel.SetRandomDirection();
        }

        private void HandleRightTapped(Object sender, RightTappedRoutedEventArgs args)
        {
            // Check to see if this kind of device is being ignored
            if (!IsValidDevice(args.PointerDeviceType)) return;

            // Examine the current position
            var position = args.GetPosition(_eventSourceElement);
            System.Diagnostics.Debug.WriteLine("RightTapped at X={0}, Y={1}", position.X, position.Y);

            // Alter the shape based on the gesture performed
            _shapeModel.ResetSizeColorAndRotation();
        }

        private void HandleHolding(Object sender, HoldingRoutedEventArgs args)
        {
            // Note that the mouse is excluded from this event...

            // Check to see if this kind of device is being ignored
            if (!IsValidDevice(args.PointerDeviceType)) return;

            // Examine the current position and holding state
            var position = args.GetPosition(_eventSourceElement);
            System.Diagnostics.Debug.WriteLine("Holding {2} at X={0}, Y={1}", position.X, position.Y, args.HoldingState);
        }

        #endregion

        private Boolean IsValidDevice(PointerDeviceType deviceType)
        {
            var supportedDevices = _shapeModel.SupportedDeviceTypes;
            return supportedDevices.Contains(deviceType);
        }
    }
}