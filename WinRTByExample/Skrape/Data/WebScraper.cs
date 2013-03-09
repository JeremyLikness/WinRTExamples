// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebScraper.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The web scraper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Skrape.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Skrape.Contracts;

    /// <summary>
    /// The web scraper.
    /// </summary>
    public class WebScraper : IWebScraper 
    {
        /// <summary>
        /// The image tag.
        /// </summary>
        public const string ImageTag = @"<(img)\b[^>]*>";

        /// <summary>
        /// The image source.
        /// </summary>
        public const string ImageSource = @"src=[\""\']([^\""\']+)";

        /// <summary>
        /// The regular expression to extract image tags
        /// </summary>
        private static readonly Regex Tags = new Regex(ImageTag, RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        /// The regular expression to extract the source attribute.
        /// </summary>
        private static readonly Regex SourceAttribute = new Regex(ImageSource, RegexOptions.IgnoreCase);

        /// <summary>
        /// The regular expression to extract the title 
        /// </summary>
        private static readonly Regex TitlePattern =
            new Regex(
                ".*<title>(.*?)</title>.*",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        /// The get html for web page.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to run asynchronously.
        /// </returns>
        public async Task GetHtmlForWebPage(SkrapedPage page)
        {
            var client = new HttpClient();
            try
            {
                var html = await client.GetStringAsync(page.Url);
                page.Html = html;

                var titleMatches = TitlePattern.Matches(html);
                foreach (
                    var match in titleMatches.Cast<Match>()
                    .Where(match => match.Groups.Count > 0))
                {
                    page.Title = match.Groups[1].Value;
                    break;
                }

                page.Text = await ParseHtmlToText(page.Html);

                var images = (await ExtractImagesFromPage(page.Html)).ToArray();

                if (images.Length <= 0)
                {
                    return;
                }

                page.Images.Clear();
                foreach (var image in images)
                {
                    page.Images.Add(image);
                }
            }
            catch (Exception ex)
            {
                page.Html = page.Text = ex.Message;
            }
        }

        /// <summary>
        /// Extract images from the HTML page and return the list of URIs
        /// </summary>
        /// <param name="content">The HTML content to parse</param>
        /// <returns>The list of images</returns>        
        private static Task<IEnumerable<Uri>> ExtractImagesFromPage(string content)
        {
            return Task.Run(
                () =>
                    {
                        var images = new List<Uri>();
                        var matches = Tags.Matches(content);
                        var max = matches.Count;

                        for (var i = 0; i < max; i++)
                        {
                            var imageTags = SourceAttribute.Matches(matches[i].Value);

                            if (imageTags.Count <= 0)
                            {
                                continue;
                            }

                            var tag = imageTags[0].Value;

                            if (string.IsNullOrEmpty(tag))
                            {
                                continue;
                            }

                            var startPos = tag.IndexOf("htt", StringComparison.Ordinal);

                            if (startPos <= 0)
                            {
                                continue;
                            }

                            Uri image;

                            if (Uri.TryCreate(tag.Substring(startPos), UriKind.Absolute, out image))
                            {
                                images.Add(image);
                            }
                        }

                        return images.Distinct();            
                    });            
        }

        /// <summary>
        /// Parse HTML to text
        /// </summary>
        /// <param name="content">The HTML content</param>
        /// <returns>The text from the HTML content</returns>        
        private static Task<string> ParseHtmlToText(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return Task.FromResult(string.Empty);
            }

            return Task.Run(() =>
                {
                    var newLine = string.Format("{0}{0}", Environment.NewLine);

                    var sb = new StringBuilder();
                    var waitForBody = true;
                    var inScriptTag = false;
                    foreach (var line in content.Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var lower = line.ToLower();

                        if (waitForBody)
                        {
                            if (!lower.Contains("<body"))
                            {
                                continue;
                            }

                            waitForBody = false;
                        }

                        if (lower.Contains("<script") || lower.Contains("<style"))
                        {
                            inScriptTag = true;
                        }

                        if (inScriptTag)
                        {
                            if (!lower.Contains("</script") && !lower.Contains("</style"))
                            {
                                continue;
                            }

                            inScriptTag = false;
                        }

                        if (lower.Contains("<br>") || lower.Contains("<br/>") || lower.Contains("<p>"))
                        {
                            sb.Append(newLine);
                        }

                        var lineStripped =
                            Regex.Replace(
                                line.Replace("<br>", newLine)
                                .Replace("<br/>", newLine)
                                .Replace("<BR>", newLine)
                                .Replace("<BR/>", newLine)
                                .Replace("<p>", newLine)
                                .Replace("<P>", newLine), 
                                "<.*?>", 
                                string.Empty);

                        if (!string.IsNullOrWhiteSpace(lineStripped))
                        {
                            sb.AppendLine(lineStripped);
                        }
                    }

                    return sb.ToString();
                });            
        }
    }
}
