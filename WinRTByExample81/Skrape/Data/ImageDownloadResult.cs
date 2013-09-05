// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageDownloadResult.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Result of image download
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Skrape.Data
{
    /// <summary>
    /// Result of image download
    /// </summary>
    public class ImageDownloadResult
    {
        /// <summary>
        /// Gets or sets the extension of the image
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets the buffer of image bytes
        /// </summary>
        public byte[] Buffer { get; set; }
    }
}
