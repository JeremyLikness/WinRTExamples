// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleAnimationType.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The multiple animation type.
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
    public class MultipleAnimationType : BaseAnimationType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationsExample.MultipleAnimationType"/> class. 
        /// </summary>
        public MultipleAnimationType()
        {
            this.Description = "Multiple Animation";
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
            var colorAnimation = new ColorAnimationType();
            var bounceEase = new BounceEaseType();
            var doubleAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            Storyboard.SetTarget(doubleAnimation, target);
            Storyboard.SetTargetProperty(
                doubleAnimation,
                "(FrameworkElement.Opacity)");
            var storyboard = new Storyboard();
            storyboard.Children.Add(colorAnimation.GenerateAnimation(target));
            storyboard.Children.Add(bounceEase.GenerateAnimation(target));
            storyboard.Children.Add(doubleAnimation);
            return storyboard;
        }
    }
}