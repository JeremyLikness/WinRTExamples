// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwipeOutThemeType.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The Swipe-out theme type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnimationsExample
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// The bounce ease type.
    /// </summary>
    public class SwipeOutThemeType : BaseAnimationType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwipeOutThemeType"/> class. 
        /// </summary>
        public SwipeOutThemeType()
        {
            this.Description = "Swipe-back Theme";
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
            var theme = new SwipeBackThemeAnimation();
            Storyboard.SetTarget(theme, target);
            var storyboard = new Storyboard();
            storyboard.Children.Add(theme);
            return storyboard;
        }
    }
}