// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrafficControl.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The traffic light control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CustomControlsLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;

    /// <summary>
    /// The traffic light.
    /// </summary>
    public sealed class TrafficControl : Control
    {
        /// <summary>
        /// The light speed property.
        /// </summary>
        public static readonly DependencyProperty LightSpeedProperty =
            DependencyProperty.Register(
            "LightSpeed",
            typeof(TimeSpan),
            typeof(TrafficControl),
            new PropertyMetadata(TimeSpan.FromSeconds(0.5)));

        /// <summary>
        /// The states.
        /// </summary>
        private readonly List<TrafficState> states = new List<TrafficState>
                                                         {
                                                             TrafficState.Green, 
                                                             TrafficState.Yellow, 
                                                             TrafficState.Red
                                                         };

        /// <summary>
        /// The current state.
        /// </summary>
        private TrafficState currentState;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrafficControl"/> class.
        /// </summary>
        public TrafficControl()
        {
            this.DefaultStyleKey = typeof(TrafficControl);
            this.currentState = TrafficState.Off;
            this.Loaded += this.TrafficControlLoaded;
        }

        /// <summary>
        /// Gets or sets the light speed.
        /// </summary>
        public TimeSpan LightSpeed
        {
            get
            {
                return (TimeSpan)this.GetValue(LightSpeedProperty);
            }

            set
            {
                this.SetValue(LightSpeedProperty, value);
            }
        }

        /// <summary>
        /// The traffic light loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void TrafficControlLoaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Off", false);

            var mainLight = this.GetTemplateChild("MainLight") as Grid;

            if (mainLight == null)
            {
                return;
            }

            mainLight.Tapped += this.MainLightTapped;
            mainLight.DoubleTapped += this.MainLightDoubleTapped;
        }

        /// <summary>
        /// The main light double tapped.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void MainLightDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            this.TransitionToState(this.currentState == TrafficState.Off
                ? TrafficState.Green : TrafficState.Off);
        }

        /// <summary>
        /// The main light tapped.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void MainLightTapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.currentState == TrafficState.Off)
            {
                return;
            }

            var idx = this.states.IndexOf(this.currentState);
            idx = ++idx % this.states.Count();
            this.TransitionToState(this.states[idx]);
        }

        /// <summary>
        /// The transition to state method.
        /// </summary>
        /// <param name="newState">
        /// The new state.
        /// </param>
        private void TransitionToState(TrafficState newState)
        {
            this.currentState = newState;
            VisualStateManager.GoToState(this, Enum.GetName(typeof(TrafficState), newState), true);
        }
    }
}
