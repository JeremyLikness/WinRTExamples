// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Is.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The is extension for tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FluentTestHelper
{
    using System;

    /// <summary>
    /// The is.
    /// </summary>
    public static class Is
    {
        /// <summary>
        /// Gets the is false.
        /// </summary>
        /// <returns>
        /// The predicate that checks for false
        /// </returns>
        public static Func<bool, bool> False
        {
            get
            {
                return target => target.IsFalse();
            }
        }

        /// <summary>
        /// Gets the is false.
        /// </summary>
        /// <returns>
        /// The predicate that checks for false
        /// </returns>
        public static Func<bool, bool> True
        {
            get
            {
                return target => target;
            }
        }

        /// <summary>
        /// The is false.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// True if value is false
        /// </returns>
        public static bool IsFalse(this bool value)
        {
            return value == false;
        }

        /// <summary>
        /// The is true.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// True if value is true
        /// </returns>
        public static bool IsTrue(this bool value)
        {
            return value;
        }

        /// <summary>
        /// The is greater than.
        /// </summary>
        /// <param name="comparison">
        /// The comparison.
        /// </param>
        /// <typeparam name="T">The type of objects to compare
        /// </typeparam>
        /// <returns>
        /// The predicate to compare for greater than
        /// </returns>
        public static Func<T, bool> GreaterThan<T>(T comparison) where T : IComparable
        {
            return target => target.IsGreaterThan(comparison);
        }

        /// <summary>
        /// Is greater than comparison
        /// </summary>
        /// <typeparam name="T">The type to compare</typeparam>
        /// <param name="value">The value</param>
        /// <param name="comparison">The value to compare to</param>
        /// <returns>True if the value is greater than the comparison</returns>
        public static bool IsGreaterThan<T>(this T value, T comparison) where T : IComparable
        {
            return value.CompareTo(comparison) > 0;
        }

        /// <summary>
        /// The not null or whitespace.
        /// </summary>
        /// <returns>
        /// The predicate to check for null or whitespace
        /// </returns>
        public static Func<string, bool> NotNullOrWhitespace()
        {
            return target => !string.IsNullOrEmpty(target);
        }

        /// <summary>
        /// The not null.
        /// </summary>
        /// <typeparam name="T">The type to check
        /// </typeparam>
        /// <returns>
        /// The predicate to check that it is not null
        /// </returns>
        public static Func<T, bool> NotNull<T>() where T : class
        {
            return target => target != null;
        }

        /// <summary>
        /// The is equal to.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <typeparam name="T">The type to compare
        /// </typeparam>
        /// <returns>
        /// The predicate to make the comparison
        /// </returns>
        public static Func<T, bool> EqualTo<T>(T other) where T : IComparable
        {
            return target => target.IsEqualTo(other);
        }

        /// <summary>
        /// The is equal to.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <typeparam name="T">The type to compare
        /// </typeparam>
        /// <returns>
        /// True when both values are equal
        /// </returns>
        public static bool IsEqualTo<T>(this T value, T other) where T : IComparable
        {
            return value.CompareTo(other) == 0;
        }
    }
}