// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FadeOutThemeType.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The fade-out theme type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnimationsExample
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// The bounce ease type.
    /// </summary>
    public class FadeOutThemeType : BaseAnimationType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FadeOutThemeType"/> class. 
        /// </summary>
        public FadeOutThemeType()
        {
            this.Description = "Fade-out Theme";
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
            var fadeOutTheme = new FadeOutThemeAnimation();
            Storyboard.SetTarget(fadeOutTheme, target);
            var storyboard = new Storyboard();
            storyboard.Children.Add(fadeOutTheme);
            return storyboard;
        }
    }
}