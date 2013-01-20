// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyComponent.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The uri loaded.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ExampleCSharpClass
{
    using System;

    /// <summary>
    /// The uri loaded.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    public delegate void UriLoaded(object sender, UriLoadedArgs args);

    /// <summary>
    /// The my component.
    /// </summary>
    public sealed class MyComponent
    {
        /// <summary>
        /// The uri loaded.
        /// </summary>
        public event UriLoaded UriLoaded;

        /// <summary>
        /// The uri to string.
        /// </summary>
        /// <param name="endPoint">
        /// The end point.
        /// </param>
        public void UriToString(Uri endPoint)
        {
            var handler = this.UriLoaded;

            if (handler != null)
            {
                handler(this, new UriLoadedArgs { Uri = endPoint.ToString() });
            }
        }
    }
}
