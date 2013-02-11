// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorAnimationType.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The color animation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnimationsExample
{
    using System;

    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// The bounce ease type.
    /// </summary>
    public class ColorAnimationType : BaseAnimationType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorAnimationType"/> class. 
        /// </summary>
        public ColorAnimationType()
        {
            this.Description = "Color Animation";
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
            var colorAnimation = new ColorAnimation
            {
                From = Colors.Red,
                To = Colors.Blue,
                Duration = TimeSpan.FromSeconds(5)                
            };

            Storyboard.SetTarget(colorAnimation, target);
            Storyboard.SetTargetProperty(
                colorAnimation,
                "(FrameworkElement.Fill).(SolidColorBrush.Color)");
            var storyboard = new Storyboard();           
            storyboard.Children.Add(colorAnimation);
            return storyboard;
        }
    }
}