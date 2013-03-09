// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWebScraper.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The WebScraper interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Skrape.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Skrape.Data;

    /// <summary>
    /// The WebScraper interface.
    /// </summary>
    public interface IWebScraper
    {
        /// <summary>
        /// Get HTML for web page.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The HTML.
        /// </returns>
        Task GetHtmlForWebPage(SkrapedPage page);        
    }
}
