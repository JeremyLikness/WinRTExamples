// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WordSplitter.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The word splitter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MyLibrary
{
    using System.Linq;

    /// <summary>
    /// The word splitter.
    /// </summary>
    public sealed class WordSplitter
    {
        /// <summary>
        /// The split.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="string"></see> array
        /// </returns>
        public string[] Split(string source)
        {
            return (from word in source.Split(' ') 
                    where !string.IsNullOrEmpty(word) 
                    orderby word 
                    select word).ToArray();
        }
    }
}
