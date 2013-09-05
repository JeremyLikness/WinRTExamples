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
    using System.Threading.Tasks;

    using Data;

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
