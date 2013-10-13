// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Has.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Defines the Has type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FluentTestHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The has.
    /// </summary>
    public static class Has
    {
        /// <summary>
        /// The only.
        /// </summary>
        /// <param name="rule">
        /// The rule.
        /// </param>
        /// <typeparam name="T">The type of the collection to check
        /// </typeparam>
        /// <typeparam name="TItem">The type of each item in the collection
        /// </typeparam>
        /// <returns>
        /// The predicate the checks the list for any exceptions
        /// </returns>
        public static Func<T, bool> Only<T, TItem>(Predicate<TItem> rule) where T : IEnumerable<TItem>
        {
            return target => target.All(item => rule(item));           
        }

        /// <summary>
        /// The any.
        /// </summary>
        /// <param name="rule">
        /// The rule.
        /// </param>
        /// <typeparam name="T">The type of the collection
        /// </typeparam>
        /// <typeparam name="TItem">The type of the item in the collection
        /// </typeparam>
        /// <returns>
        /// The predicate that checks to see if any item exists
        /// </returns>
        public static Func<T, bool> Any<T, TItem>(Predicate<TItem> rule) where T : IEnumerable<TItem>
        {
            return target => target.Any(item => rule(item));
        }

        /// <summary>
        /// The none.
        /// </summary>
        /// <param name="rule">
        /// The rule.
        /// </param>
        /// <typeparam name="T">The type of the collection
        /// </typeparam>
        /// <typeparam name="TItem">The type of the item in the collection
        /// </typeparam>
        /// <returns>
        /// The predicate that checks to make sure no items for the rule exist
        /// </returns>
        public static Func<T, bool> None<T, TItem>(Predicate<TItem> rule) where T : IEnumerable<TItem>
        {
            return target => !target.Any(item => rule(item));
        }
    }
}
