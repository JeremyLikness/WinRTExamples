using System;
using System.Linq;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace InputsExample
{
    // TODO - Relay events to log/display area
    // TODO - figure out keyboard input issue

    public class InputEventHandler
    {
        private ShapeModel _shapeModel;

        public InputEventHandler(FrameworkElement element)
        {
            if (element == null) throw new ArgumentNullException("element");
            element.DataContextChanged += (sender, args) =>
                                          {
                                              _shapeModel = args.NewValue as ShapeModel;
                                          };

            element.PointerEntered += HandlePointerEntered;
            element.PointerExited += HandlePointerExited;
            element.PointerPressed += HandlePointerPressed;
            element.PointerMoved += HandlePointerMoved;
            element.PointerReleased += HandlePointerReleased;
            //element.PointerCanceled 
            //element.PointerCaptureLost

            element.ManipulationStarting += HandleManipulationStarting;
            element.ManipulationStarted += HandleManipulationStarted;
            element.ManipulationDelta += HandleManipulationDelta;
            element.ManipulationCompleted += HandleManipulationCompleted;
            element.ManipulationInertiaStarting += HandleManipulationInertiaStarting;

            element.Tapped += HandleTapped;
            element.DoubleTapped += HandleDoubleTapped;
            element.RightTapped += HandleRightTapped;
            element.Holding += HandleHolding;
        }

        #region Pointer

        private void HandlePointerEntered(Object sender, PointerRoutedEventArgs args)
        {
            if (!_shapeModel.SupportedDeviceTypes.Contains(args.Pointer.PointerDeviceType))
            {
                args.Handled = true;
                return;
            }

            _shapeModel.IsHot = true;
        }

        private void HandlePointerExited(Object sender, PointerRoutedEventArgs args)
        {
            _shapeModel.IsHot = false;
        }

        private void HandlePointerPressed(Object sender, PointerRoutedEventArgs args)
        {
            // TODO - Perhaps Update the shape to show the pointer ID?
            // Getting the pointer coordinates relative to ?
            //var point = args.GetCurrentPoint(foo);

            // Getting the pointer id
            // args.Pointer.PointerId

            
            
            
            
            // Determining if a stylus is hovering or actually touching...
            // args.Pointer.IsInContact
            // args.Pointer.IsInRange
        }

        private void HandlePointerMoved(Object sender, PointerRoutedEventArgs args)
        {
        }

        private void HandleHolding(Object sender, HoldingRoutedEventArgs args)
        {
            // Note that the mouse is excluded from this event...
            if (args.HoldingState == HoldingState.Started) _shapeModel.IsHot = true;
            else if (args.HoldingState == HoldingState.Completed) _shapeModel.IsHot = false;
        }

        private void HandlePointerReleased(Object sender, PointerRoutedEventArgs args)
        {
        }

        #endregion

        #region Manipulations

        private void HandleManipulationStarting(Object sender, ManipulationStartingRoutedEventArgs args)
        {
            // Set the current manipulation mode
            args.Mode = _shapeModel.ManipulationMode;
        }

        private void HandleManipulationStarted(Object sender, ManipulationStartedRoutedEventArgs args)
        {
            if (!_shapeModel.SupportedDeviceTypes.Contains(args.PointerDeviceType))
            {
                args.Complete();
            }
        }

        private void HandleManipulationDelta(Object sender, ManipulationDeltaRoutedEventArgs args)
        {
            var delta = args.Delta;
            _shapeModel.MoveShape(delta.Translation.X, delta.Translation.Y);
            _shapeModel.ResizeShape(delta.Scale);
            _shapeModel.RotateShape(delta.Rotation);
        }

        private void HandleManipulationInertiaStarting(Object sender, ManipulationInertiaStartingRoutedEventArgs args)
        {
        }

        private void HandleManipulationCompleted(Object sender, ManipulationCompletedRoutedEventArgs args)
        {
            // Velocity here is returned as Pixel/ms
            var xRate = args.Velocities.Linear.X * 30;
            var yRate = args.Velocities.Linear.Y * 30;
            _shapeModel.UpdateRate(xRate, yRate);
        }

        #endregion

        #region Gestures

        private void HandleTapped(Object sender, TappedRoutedEventArgs args)
        {
            if (!_shapeModel.SupportedDeviceTypes.Contains(args.PointerDeviceType)) return;
            _shapeModel.SetRandomColor();
        }

        private void HandleDoubleTapped(Object sender, DoubleTappedRoutedEventArgs args)
        {
            if (!_shapeModel.SupportedDeviceTypes.Contains(args.PointerDeviceType)) return;
            _shapeModel.SetRandomDirection();
        }

        private void HandleRightTapped(Object sender, RightTappedRoutedEventArgs args)
        {
            if (!_shapeModel.SupportedDeviceTypes.Contains(args.PointerDeviceType)) return;
            // TODO - reset shape to initial values
        }

        #endregion
    }
}