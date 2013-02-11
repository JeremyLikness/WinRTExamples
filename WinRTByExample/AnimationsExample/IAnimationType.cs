// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAnimationType.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The AnimationType interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnimationsExample
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// The AnimationType interface.
    /// </summary>
    public interface IAnimationType
    {
        /// <summary>
        /// Gets the id.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The start animation.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="Storyboard"/>.
        /// </returns>
        Storyboard GenerateAnimation(FrameworkElement target);
    }
}