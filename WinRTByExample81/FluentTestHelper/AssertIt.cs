// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssertIt.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Defines the AssertIt type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FluentTestHelper
{
    using System;

    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    /// <summary>
    /// The test extensions.
    /// </summary>
    public static class AssertIt
    {
        /// <summary>
        /// The it.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <typeparam name="T">The type of the item to perform the assertion against
        /// </typeparam>
        public static void That<T>(T target, Func<T, bool> condition, string message = "") 
        {
                if (string.IsNullOrWhiteSpace(message))
                {
                    Assert.IsTrue(condition(target));
                }
                else
                {
                    Assert.IsTrue(condition(target), message);
                }
        }       
    }
}