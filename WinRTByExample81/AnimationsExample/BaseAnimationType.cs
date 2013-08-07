// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseAnimationType.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The base animation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AnimationsExample
{
    using System.Threading;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// The base animation type.
    /// </summary>
    public abstract class BaseAnimationType : IAnimationType
    {
        /// <summary>
        /// The id.
        /// </summary>
        private static int id;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAnimationType"/> class.
        /// </summary>
        protected BaseAnimationType()
        {
            this.Id = Interlocked.Add(ref id, 1);
        }       

        /// <summary>
        /// Gets the id.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public abstract string Description { get; protected set; }

        /// <summary>
        /// The start animation.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="Storyboard"/>.
        /// </returns>
        public abstract Storyboard GenerateAnimation(FrameworkElement target);
    }
}