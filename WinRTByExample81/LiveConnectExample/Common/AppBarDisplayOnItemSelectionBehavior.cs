using System;
using System.Linq;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Xaml.Interactivity;

namespace LiveConnectExample
{
    public class AppBarDisplayOnItemSelectionBehavior : DependencyObject, IBehavior
    {
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
                var selector = AssociatedObject as Selector;
                if (selector != null)
                {
                    HookEvents(selector);
                }
            }
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            var selector = AssociatedObject as Selector;
            if (selector != null)
            {
                UnhookEvents(selector);
            }
        }

        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> is attached.
        /// </summary>
        public DependencyObject AssociatedObject
        {
            get; private set;
        }

        public AppBarDisplayFlags DisplayOnSelectionAction { get; set; }


        private void HookEvents(Selector selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");

            // Clear any "active" event handlers  
            UnhookEvents(selector);

            if (DisplayOnSelectionAction != AppBarDisplayFlags.None)
            {
                selector.Unloaded += HandleUnloaded;
                selector.SelectionChanged += HandleSelectionChanged;
            }
        }

        private void UnhookEvents(Selector selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");

            selector.Unloaded -= HandleUnloaded;
            selector.SelectionChanged -= HandleSelectionChanged;
        }

        private void HandleUnloaded(Object sender, RoutedEventArgs e)
        {
            UnhookEvents((Selector)sender);
        }

        private void HandleSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            var selector = sender as Selector;
            if (selector == null) return;
            // Traverse the selector's parents to find the first "page" element  
            var containingPage = selector.GetVisualAncestors().OfType<Page>().FirstOrDefault();
            if (containingPage == null) return;


            var currentFlags = DisplayOnSelectionAction;
            var showBottomAppBar = (currentFlags & AppBarDisplayFlags.Bottom) == AppBarDisplayFlags.Bottom;
            var showTopAppBar = (currentFlags & AppBarDisplayFlags.Top) == AppBarDisplayFlags.Top;
            if (selector.SelectedItem != null)
            {
                // An item has been selected - show the relevant app bars  
                if (showBottomAppBar) ShowAppBar(containingPage.BottomAppBar);
                if (showTopAppBar) ShowAppBar(containingPage.TopAppBar);
            }
            else
            {
                // Nothing has been selected - hide the relevant app bars  
                if (showBottomAppBar) HideAppBar(containingPage.BottomAppBar);
                if (showTopAppBar) HideAppBar(containingPage.TopAppBar);
            }
        }

        private void ShowAppBar(AppBar appBar)
        {
            if (appBar == null) return;

            appBar.IsSticky = true;
            appBar.IsOpen = true;
        }

        private void HideAppBar(AppBar appBar)
        {
            if (appBar == null) return;

            appBar.IsOpen = false;
            appBar.IsSticky = false;
        }

        [Flags]
        public enum AppBarDisplayFlags
        {
            None = 0,
            Bottom = 1,
            Top = 2,
            BottomAndTop = 3,
        }
    }
}