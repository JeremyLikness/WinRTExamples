// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PowerEaseType.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The power ease type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnimationsExample
{
    using System;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// The bounce ease type.
    /// </summary>
    public class PowerEaseType : BaseAnimationType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationsExample.PowerEaseType"/> class. 
        /// </summary>
        public PowerEaseType()
        {
            this.Description = "Power Ease";
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public override sealed string Description { get; protected set; }

        /// <summary>
        /// The start animation.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="Storyboard"/>.
        /// </returns>
        public override Storyboard GenerateAnimation(FrameworkElement target)
        {
            var ease = new PowerEase
            {
                EasingMode = EasingMode.EaseInOut
            };

            var doubleAnimation = new DoubleAnimation
            {
                From = 0,
                To = 800,
                Duration = TimeSpan.FromSeconds(5),
                EasingFunction = ease
            };
            Storyboard.SetTarget(doubleAnimation, target);
            Storyboard.SetTargetProperty(
                doubleAnimation,
                "(FrameworkElement.RenderTransform).(TranslateTransform.X)");
            var storyboard = new Storyboard();
            storyboard.Children.Add(doubleAnimation);
            return storyboard;
        }
    }
}