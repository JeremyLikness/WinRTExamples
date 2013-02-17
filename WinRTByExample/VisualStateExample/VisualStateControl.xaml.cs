// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualStateControl.xaml.cs" company="Jeremy Liknesss">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The visual state control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VisualStateExample
{
    using System;
    using System.Diagnostics;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;

    /// <summary>
    /// The visual state control.
    /// </summary>
    public sealed partial class VisualStateControl
    {
        /// <summary>
        /// The stretched.
        /// </summary>
        private bool stretched;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStateControl"/> class.
        /// </summary>
        public VisualStateControl()
        {
            this.InitializeComponent();
            this.Loaded += this.VisualStateControlLoaded;
        }

        /// <summary>
        /// The group current state changing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public static void GroupCurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            Debug.WriteLine(
                "{2} changing from {0} to {1}",
                e.OldState == null ? "*" : e.OldState.Name,
                e.NewState == null ? "*" : e.NewState.Name,
                DateTime.Now);
        }

        /// <summary>
        /// The group current state changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public static void GroupCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            Debug.WriteLine(
                "{2} changed from {0} to {1}",
                e.OldState == null ? "*" : e.OldState.Name,
                e.NewState == null ? "*" : e.NewState.Name,
                DateTime.Now);
        }

        /// <summary>
        /// The visual state control loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void VisualStateControlLoaded(object sender, RoutedEventArgs e)
        {
            this.SetupTroubleshooting();
            VisualStateManager.GoToState(this, "Default", false);
        }

        /// <summary>
        /// The setup troubleshooting.
        /// </summary>
        private void SetupTroubleshooting()
        {
            var groups = VisualStateManager.GetVisualStateGroups(this.Content as Grid);
            foreach (var @group in groups)
            {
                Debug.WriteLine("Group {0}", @group.Name);
                group.CurrentStateChanged += GroupCurrentStateChanged;
                group.CurrentStateChanging += GroupCurrentStateChanging;
                foreach (var state in @group.States)
                {
                    Debug.WriteLine("   with State {0}", state.Name);
                }

                foreach (var transition in @group.Transitions)
                {
                    Debug.WriteLine(
                        "   with transition {0} to {1}",
                        string.IsNullOrEmpty(transition.From) ? "*" : transition.From,
                        string.IsNullOrEmpty(transition.To) ? "*" : transition.To);
                }
            }
        }

        /// <summary>
        /// The main ellipse_ on tapped.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainEllipse_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, this.stretched ? "Default" : "Stretched", true);
            this.stretched = !this.stretched;
        }        
    }
}
