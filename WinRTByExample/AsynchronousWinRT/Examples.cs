// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Examples.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The examples.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace AsynchronousWinRT
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading;
    using System.Threading.Tasks;

    using Windows.Foundation;

    /// <summary>
    /// The examples.
    /// </summary>
    public sealed class Examples
    {
        /// <summary>
        /// The add numbers.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <returns>
        /// The asynchronous operation
        /// </returns>
        public IAsyncOperation<long> AddNumbers([ReadOnlyArray] int[] array)
        {
            return AddNumbersInternal(array).AsAsyncOperation();
        }

        /// <summary>
        /// The add numbers with progress.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <returns>
        /// The asynchronous operation.
        /// </returns>
        public IAsyncOperationWithProgress<long, double> 
            AddNumbersWithProgress([ReadOnlyArray] int[] array)
        {
            return AsyncInfo.Run(
                async (
                    CancellationToken cancellationToken, 
                    IProgress<double> progress) =>
                    {
                        progress.Report(0);
                        return await Task.Run(
                            () =>
                                {
                                    long result = 0;
                                    for (var index = 0; 
                                        index < array.Length; 
                                        index++)
                                    {
                                        progress.Report(
                                            (double)index / 
                                            array.Length);
                                        result += index;
                                    }

                                    return result;
                                });
                    });
        }

        /// <summary>
        /// The add numbers.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static Task<long> AddNumbersInternal(ICollection<int> array)
        {
            return Task.Run(
                () =>
                    {
                        long result = 0;
                        for (var index = 0; index < array.Count; index++)
                        {
                            result += index;
                        }

                        return result;
                    });
        }
    }
}