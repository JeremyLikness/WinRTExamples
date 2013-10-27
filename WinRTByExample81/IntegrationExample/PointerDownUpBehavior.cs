using System;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Xaml.Interactivity;

namespace IntegrationExample
{
    public class PointerDownUpBehavior : DependencyObject, IBehavior
    {
        private Storyboard _pointerDownStoryboard;
        private Storyboard _pointerUpStoryboard;

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">The <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> will be attached.</param>
        public void Attach(DependencyObject associatedObject)
        {
            if ((associatedObject != AssociatedObject) && !DesignMode.DesignModeEnabled)
            {
                if (AssociatedObject != null) throw new InvalidOperationException("Cannot attach behavior multiple times.");

                AssociatedObject = associatedObject;
                var frameworkElement = AssociatedObject as FrameworkElement;
                if (frameworkElement != null)
                {
                    _pointerDownStoryboard = new Storyboard();
                    _pointerDownStoryboard.Children.Add(new PointerDownThemeAnimation());
                    Storyboard.SetTarget(_pointerDownStoryboard, frameworkElement);

                    _pointerUpStoryboard = new Storyboard();
                    _pointerUpStoryboard.Children.Add(new PointerUpThemeAnimation());
                    Storyboard.SetTarget(_pointerUpStoryboard, frameworkElement);

                    frameworkElement.PointerPressed += OnPointerPressed;
                    frameworkElement.PointerReleased += OnPointerReleased;
                }
            }
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            var frameworkElement = AssociatedObject as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.PointerPressed -= OnPointerPressed;
                frameworkElement.PointerReleased -= OnPointerReleased;
            }

            AssociatedObject = null;
        }

        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> is attached.
        /// </summary>
        public DependencyObject AssociatedObject { get; private set; }

        private void OnPointerPressed(Object sender, PointerRoutedEventArgs e)
        {
            _pointerDownStoryboard.Begin();
        }

        private void OnPointerReleased(Object sender, PointerRoutedEventArgs e)
        {
            _pointerUpStoryboard.Begin();
        }
    }
}